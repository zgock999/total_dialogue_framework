using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using TotalDialogue.Core.Collections;
using UnityEngine;
using UnityEngine.TestTools;


namespace TotalDialogue.Tests
{
    public class ObservableListPlayTests
    {
        [Test]
        public void Test_CollectionChangedAction_IsTriggered_When_ItemAdded()
        {
            // Arrange
            var list = new TDFList<int>();
            Assert.IsTrue(list != null);
            bool onChangeBool = false;
            int onAddIndex = -1;
            int onAddInt = 0;
            list.OnChange += () => onChangeBool = true;
            list.OnAdd += (index,item) => { onAddIndex = index; onAddInt = item; };

            // Act
            list.Add(1);

            // Assert
            Assert.IsTrue(list.Count == 1);
            Assert.IsTrue(list[0] == 1);
            Assert.IsTrue(onChangeBool);
            Assert.IsTrue(onAddIndex == 0);
            Assert.IsTrue(onAddInt == 1);

        }
        [Test]
        public void Test_CollectionChangedAction_IsTriggered_When_ItemChanged()
        {
            // Arrange
            var list = new TDFList<int>();
            bool onChangeBool = false;
            list.OnChange += () => onChangeBool = true;
            int onChangeInt = 0;
            int onChangeIndex = -1;
            list.OnChangeItem += (index, item) => { onChangeInt = item; onChangeIndex = index; };
            int onAddIndex = -1;
            int onAddInt = 0;
            list.OnAdd += (index,item) => { onAddIndex = index; onAddInt = item; };

            // Act
            list.Add(1);
            list[0] = 2;
            list.Add(3);

            // Assert
            Assert.IsTrue(onChangeBool);
            Assert.IsTrue(onChangeIndex == 0);
            Assert.IsTrue(onChangeInt == 2);
            Assert.IsTrue(onAddIndex == 1);
            Assert.IsTrue(onAddInt == 3);

            // Act
            list.Add(1);
            list[1] += 1;
            list.Add(3);
            Assert.IsTrue(onChangeIndex == 1);
            Assert.IsTrue(onChangeInt == 4);


        }
        [Test]
        public void Test_CollectionChangedAction_IsTriggered_When_ItemInsert()
        {
            // Arrange
            var list = new TDFList<int>();
            int onAddIndex = -1;
            int onAddInt = 0;
            list.OnAdd += (index,item) => { onAddIndex = index; onAddInt = item; };
            // Act
            list.Add(1);
            list.Insert(0,2);

            // Assert
            Assert.IsTrue(onAddIndex == 0);
            Assert.IsTrue(onAddInt == 2);
        }
        
        [Test]
        public void Test_CollectionChangedAction_IsTriggered_When_ItemRemove()
        {
            // Arrange
            var list = new TDFList<int>();
            int onRemoveIndex = -1;
            int onRemoveInt = -1;
            list.OnRemove += (index,item) => { onRemoveIndex = index; onRemoveInt = item; };
            // Act
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Remove(0);

            // Assert
            Assert.IsTrue(onRemoveIndex == -1);
            Assert.IsTrue(onRemoveInt == -1);

            // Act
            list.Remove(1);

            // Assert
            Assert.IsTrue(onRemoveIndex == 0);
            Assert.IsTrue(onRemoveInt == 1);
            // Act
            list.RemoveAt(1);

            // Assert
            Assert.IsTrue(onRemoveIndex == 1);
            Assert.IsTrue(onRemoveInt == 3);

            bool onChangeBool = false;
            list.OnChange += () => onChangeBool = true;
            list.Clear();
            Assert.IsTrue(onChangeBool);
        }
        
        [Test]
        public void TestSerialization()
        {
            var list = new TDFList<int> { 1, 2, 3 };
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, list);


                Assert.Greater(stream.Length, 0);
            }
        }

        [Test]
        public void TestDeserialization()
        {
            var originalList = new TDFList<int> { 1, 2, 3 };
            var formatter = new BinaryFormatter();
            byte[] serialized;
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, originalList);
                serialized = stream.ToArray();
            }
            using (var stream = new MemoryStream(serialized))
            {
                var deserializedList = (TDFList<int>)formatter.Deserialize(stream);
                Assert.AreEqual(originalList.Count, deserializedList.Count);
                for (int i = 0; i < originalList.Count; i++)
                {
                    Assert.AreEqual(originalList[i], deserializedList[i]);
                }
            }
        }
        [UnityTest]
        public IEnumerator TestThreadSafety()
        {
            var observableList = new TDFList<int>();
            var tasks = new List<Task>();
            var itemCount = 1000;

            for (var i = 0; i < itemCount; i++)
            {
                var task = Task.Run(() =>
                {
                    observableList.Add(i);
                });

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            yield return null;

            Assert.AreEqual(itemCount, observableList.Count);

            tasks.Clear();
            for (var i = 0; i < itemCount; i++)
            {
                var task = Task.Run(() =>
                {
                    observableList.RemoveAt(0);
                });

                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());

            yield return null;

            Assert.AreEqual(0, observableList.Count);
        }
        [UnityTest]
        public IEnumerator TestBeginLockBlocksOtherThreads()
        {
            // Arrange
            var observableList = new TDFList<int>();
            var addRangeThreadStarted = new ManualResetEvent(false);
            var addRangeThreadCompleted = new ManualResetEvent(false);

            // Act
            var addRangeThread = new Thread(() =>
            {
                addRangeThreadStarted.Set();
                observableList.AddRange(new TDFList<int> { 1 });
                addRangeThreadCompleted.Set();
            });

            observableList.BeginLock();
            addRangeThread.Start();

            // Allow the addRangeThread to start and attempt to add an item to the list
            yield return new WaitUntil(() => addRangeThreadStarted.WaitOne(1000));

            // Assert
            Assert.IsFalse(addRangeThreadCompleted.WaitOne(1000), "AddRange thread should not have completed while the list is locked.");

            // Clean up
            observableList.EndLock();
            yield return new WaitUntil(() => addRangeThreadCompleted.WaitOne(1000));
        }
        private async UniTask AddItems<T>(TDFList<T> list, int itemCount, Func<int, T> itemGenerator)
        {
            for (int i = 0; i < itemCount; i++)
            {
                list.Add(itemGenerator(i));
                await UniTask.Yield();  // Yield after each addition to simulate concurrency
            }
        }

        private async UniTask RemoveItems<T>(TDFList<T> list, int itemCount, Func<int, T> itemGenerator)
        {
            for (int i = 0; i < itemCount; i++)
            {
                list.Remove(itemGenerator(i));
                await UniTask.Yield();  // Yield after each removal to simulate concurrency
            }
        }

        private async UniTask IndexItems<T>(TDFList<T> list, int itemCount, Func<int, T> itemGenerator)
        {
            for (int i = 0; i < itemCount; i++)
            {
                list[i] = itemGenerator(i);
                await UniTask.Yield();  // Yield after each indexing to simulate concurrency
            }
        }

        
        [UnityTest]
        public IEnumerator TestAddIsThreadSafe()
        {
            return UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var observableListInt = new TDFList<int>();
                var observableListString = new TDFList<string>();
                var observableListFloat = new TDFList<float>();
                var observableListVector3 = new TDFList<Vector3>();
                int itemCount = 1000;

                // Act
                await UniTask.WhenAll(
                    AddItems(observableListInt, itemCount, i => i),
                    AddItems(observableListString, itemCount, i => i.ToString()),
                    AddItems(observableListFloat, itemCount, i => (float)i),
                    AddItems(observableListVector3, itemCount, i => new Vector3(i, i, i))
                );

                // Assert
                Assert.AreEqual(itemCount, observableListInt.Count);
                Assert.AreEqual(itemCount, observableListString.Count);
                Assert.AreEqual(itemCount, observableListFloat.Count);
                Assert.AreEqual(itemCount, observableListVector3.Count);
            });
        }
        [UnityTest]
        public IEnumerator TestRemoveIsThreadSafe()
        {
            return UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var observableListInt = new TDFList<int>();
                var observableListString = new TDFList<string>();
                var observableListFloat = new TDFList<float>();
                var observableListVector3 = new TDFList<Vector3>();
                int itemCount = 1000;

                // Add items first
                await UniTask.WhenAll(
                    AddItems(observableListInt, itemCount, i => i),
                    AddItems(observableListString, itemCount, i => i.ToString()),
                    AddItems(observableListFloat, itemCount, i => (float)i),
                    AddItems(observableListVector3, itemCount, i => new Vector3(i, i, i))
                );

                // Act
                await UniTask.WhenAll(
                    RemoveItems(observableListInt, itemCount, i => i),
                    RemoveItems(observableListString, itemCount, i => i.ToString()),
                    RemoveItems(observableListFloat, itemCount, i => (float)i),
                    RemoveItems(observableListVector3, itemCount, i => new Vector3(i, i, i))
                );

                // Assert
                Assert.AreEqual(0, observableListInt.Count);
                Assert.AreEqual(0, observableListString.Count);
                Assert.AreEqual(0, observableListFloat.Count);
                Assert.AreEqual(0, observableListVector3.Count);
            });
        }

        [UnityTest]
        public IEnumerator TestIndexIsThreadSafe()
        {
            return UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var observableListInt = new TDFList<int>();
                var observableListString = new TDFList<string>();
                var observableListFloat = new TDFList<float>();
                var observableListVector3 = new TDFList<Vector3>();
                int itemCount = 1000;

                // Add items first
                await UniTask.WhenAll(
                    AddItems(observableListInt, itemCount, i => 0),
                    AddItems(observableListString, itemCount, i => "0"),
                    AddItems(observableListFloat, itemCount, i => 0f),
                    AddItems(observableListVector3, itemCount, i => Vector3.zero)
                );

                // Act
                await UniTask.WhenAll(
                    IndexItems(observableListInt, itemCount, i => i),
                    IndexItems(observableListString, itemCount, i => i.ToString()),
                    IndexItems(observableListFloat, itemCount, i => (float)i),
                    IndexItems(observableListVector3, itemCount, i => new Vector3(i, i, i))
                );

                // Assert
                for (int i = 0; i < itemCount; i++)
                {
                    Assert.AreEqual(i, observableListInt[i]);
                    Assert.AreEqual(i.ToString(), observableListString[i]);
                    Assert.AreEqual((float)i, observableListFloat[i]);
                    Assert.AreEqual(new Vector3(i, i, i), observableListVector3[i]);
                }
            });
        }

        [UnityTest]
        public IEnumerator TestLockingMechanism()
        {
            // Arrange
            var observableList = new TDFList<int>();
            observableList.Add(1);
            observableList.BeginLock();

            // Act
            Task addTask = Task.Run(() => observableList.Add(2));
            Task removeTask = Task.Run(() => observableList.Remove(1));
            Task indexTask = Task.Run(() => observableList[0] = 3);

            // Assert
            Assert.IsFalse(addTask.IsCompleted);
            Assert.IsFalse(removeTask.IsCompleted);
            Assert.IsFalse(indexTask.IsCompleted);

            observableList.EndLock();

            yield return new WaitForSeconds(1); // Wait for tasks to complete

            Assert.IsTrue(addTask.IsCompleted);
            Assert.IsTrue(removeTask.IsCompleted);
            Assert.IsTrue(indexTask.IsCompleted);
            Assert.AreEqual(1, observableList.Count);
            Assert.AreEqual(3, observableList[0]);
        }
        [UnityTest]
        public IEnumerator Test_CollectionChangedAction_IsTriggered_Thread_ItemRemove()
        {
            return UniTask.ToCoroutine(async () =>
            {
            // Arrange
            var list = new TDFList<int>();
            int onRemoveIndex = -1;
            int onRemoveInt = -1;
            list.OnRemove += (index, item) => { onRemoveIndex = index; onRemoveInt = item; };

            // Act
            list.Add(1);
            list.Add(2);
            list.Add(3);
            await UniTask.RunOnThreadPool(() => list.Remove(0));

            // Assert
            Assert.IsTrue(onRemoveIndex == -1);
            Assert.IsTrue(onRemoveInt == -1);

            // Act
            await UniTask.RunOnThreadPool(() => list.Remove(1));

            // Assert
            Assert.IsTrue(onRemoveIndex == 0);
            Assert.IsTrue(onRemoveInt == 1);

            // Act
            await UniTask.RunOnThreadPool(() => list.RemoveAt(1));

            // Assert
            Assert.IsTrue(onRemoveIndex == 1);
            Assert.IsTrue(onRemoveInt == 3);

            bool onChangeBool = false;
            list.OnChange += () => onChangeBool = true;
            await UniTask.RunOnThreadPool(() => list.Clear());
            Assert.IsTrue(onChangeBool);
            });
        }
        [UnityTest]
        public IEnumerator Test_ItemAdded_IsTriggered_When_ItemAdded_From_Different_Thread()
        {
            return UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var list = new TDFList<int>();
                int onAddIndex = -1;
                int onAddInt = -1;
                list.OnAdd += (index, item) => { onAddIndex = index; onAddInt = item; };

                // Act
                await UniTask.RunOnThreadPool(() => list.Add(1));

                // Assert
                Assert.IsTrue(onAddIndex == 0);
                Assert.IsTrue(onAddInt == 1);
            });
        }

        [UnityTest]
        public IEnumerator Test_ItemChanged_IsTriggered_When_ItemChanged_From_Different_Thread()
        {
            return UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var list = new TDFList<int> { 1 };
                int onChangedIndex = -1;
                int onChangedInt = -1;
                list.OnChangeItem += (index, item) => { onChangedIndex = index; onChangedInt = item; };

                // Act
                await UniTask.RunOnThreadPool(() => list[0] = 2);

                // Assert
                Assert.IsTrue(onChangedIndex == 0);
                Assert.IsTrue(onChangedInt == 2);
            });
        }

        [UnityTest]
        public IEnumerator Test_ItemInsert_IsTriggered_When_ItemInsert_From_Different_Thread()
        {
            return UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var list = new TDFList<int>();
                int onInsertIndex = -1;
                int onInsertInt = -1;
                list.OnAdd += (index, item) => { onInsertIndex = index; onInsertInt = item; };

                // Act
                await UniTask.RunOnThreadPool(() => list.Insert(0, 1));

                // Assert
                Assert.IsTrue(onInsertIndex == 0);
                Assert.IsTrue(onInsertInt == 1);
            });
        }
    }
}