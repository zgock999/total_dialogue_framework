using System;
using System.CodeDom;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using TotalDialogue.Core.Collections;


namespace TotalDialogue.Tests
{

    public class TDFListEditorTests
    {
        private class  MyClass
        {
            public int Int;
            public string String;
        }
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
        public void Test_Serialization()
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
        public void Test_Deserialization()
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
        [Test]
        public void Test_ItemType()
        {
            // Arrange
            var list1 = new TDFList<int>();
            var list2 = new TDFList<string>();
            var list3 = new TDFList<MyClass>();
 
            Type type1 = list1.GetItemType();
            Type type2 = list2.GetItemType();
            Type type3 = list3.GetItemType();
            // Assert
            Assert.IsTrue(type1 == typeof(int));
            Assert.IsTrue(type2 == typeof(string));
            Assert.IsTrue(type3 == typeof(MyClass));


        }
    }
}