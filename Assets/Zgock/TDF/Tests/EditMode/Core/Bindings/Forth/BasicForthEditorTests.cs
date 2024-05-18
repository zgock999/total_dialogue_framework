using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
namespace TotalDialogue.Tests
{
    public class BasicForthTests
    {
        [Test]
        public async void Test_TokenAction_StackOperationWorks()
        {
            string [] results;
            // Arrange
            var forth = new BasicForth();
            Assert.IsTrue(forth != null);
            Assert.AreEqual(0, forth.Stack.Count);

            // Act
            forth.Stack.Clear();
            await forth.Execute("1");
            results = forth.Stack.ToArray();

            // Assert
            Assert.AreEqual(1, forth.Stack.Count);
            Assert.AreEqual("1", results[0]);

            // Act
            await forth.Execute("2");
            results = forth.Stack.ToArray();

            // Assert
            Assert.AreEqual(2, forth.Stack.Count);
            Assert.AreEqual("2", results[0]);
            Assert.AreEqual("1", results[1]);

            // Act
            await forth.Execute("swap");
            results = forth.Stack.ToArray();

            // Assert
            Assert.AreEqual(2, forth.Stack.Count);
            Assert.AreEqual("1", results[0]);
            Assert.AreEqual("2", results[1]);

            // Act
            await forth.Execute("dup");
            results = forth.Stack.ToArray();

            // Assert
            Assert.AreEqual(3, forth.Stack.Count);
            Assert.AreEqual("1", results[0]);
            Assert.AreEqual("1", results[1]);
            Assert.AreEqual("2", results[2]);

            // Act
            await forth.Execute("drop");
            results = forth.Stack.ToArray();

            // Assert
            Assert.AreEqual(2, forth.Stack.Count);
            Assert.AreEqual("1", results[0]);
            Assert.AreEqual("2", results[1]);

            // Act
            forth.Stack.Clear();
            await forth.Execute("\"aaa\"");
            results = forth.Stack.ToArray();

            // Assert
            Assert.AreEqual(1, forth.Stack.Count);
            Assert.AreEqual("aaa", results[0]);

            // Act
            forth.Stack.Clear();
            await forth.Execute("\"aaa\tbbb\nccc\"");
            results = forth.Stack.ToArray();

            // Assert
            Assert.AreEqual(1, forth.Stack.Count);
            Assert.AreEqual("aaa\tbbb\nccc", results[0]);

            // Act
            forth.Stack.Clear();
            await forth.Execute("'aaa\\tbbb\\nccc\\\\'");
            results = forth.Stack.ToArray();

            // Assert
            Assert.AreEqual(1, forth.Stack.Count);
            Assert.AreEqual("aaa\tbbb\nccc\\", results[0]);
        }
        [Test]
        public async void Test_TokenAction_NunericOperationWorks()
        {
            string [] results;
            // Arrange
            var forth = new BasicForth();
            Assert.IsTrue(forth != null);

            // Act
            await forth.Execute("1 2");
            results = forth.Stack.ToArray();

            // Assert
            Assert.AreEqual(2, forth.Stack.Count);
            Assert.AreEqual("2", results[0]);
            Assert.AreEqual("1", results[1]);

            // Act
            await forth.Execute("+");
            results = forth.Stack.ToArray();

            // Assert
            Assert.AreEqual(1, forth.Stack.Count);
            Assert.AreEqual("3", results[0]);

            // Act
            forth.Stack.Clear();
            await forth.Execute("1 2 +");
            results = forth.Stack.ToArray();

            // Assert
            Assert.AreEqual(1, forth.Stack.Count);
            Assert.AreEqual("3", results[0]);

            // Act
            forth.Stack.Clear();
            await forth.Execute("1 2 -");
            results = forth.Stack.ToArray();

            // Assert
            Assert.AreEqual(1, forth.Stack.Count);
            Assert.AreEqual("-1", results[0]);

            // Act
            forth.Stack.Clear();
            await forth.Execute("2 2 *");
            results = forth.Stack.ToArray();

            // Assert
            Assert.AreEqual(1, forth.Stack.Count);
            Assert.AreEqual("4", results[0]);

            // Act
            forth.Stack.Clear();
            await forth.Execute("3 2 /");
            results = forth.Stack.ToArray();

            // Assert
            Assert.AreEqual(1, forth.Stack.Count);
            Assert.AreEqual("1.5", results[0]);
        }
        [Test]
        public async void Test_TokenAction_StringOperationWorks()
        {
            string [] results;
            // Arrange
            var forth = new BasicForth();
            Assert.IsTrue(forth != null);

            // Act
            forth.Stack.Clear();
            await forth.Execute("'aaa' '100' +");
            results = forth.Stack.ToArray();

            // Assert
            Assert.AreEqual(1, forth.Stack.Count);
            Assert.AreEqual("aaa100", results[0]);

            // Act
            forth.Stack.Clear();
            await forth.Execute("'1234' 'aaa' +");
            results = forth.Stack.ToArray();

            // Assert
            Assert.AreEqual(1, forth.Stack.Count);
            Assert.AreEqual("1234aaa", results[0]);

            // Act
            forth.Stack.Clear();
            await forth.Execute("'abcdef' 'bcd' -");
            results = forth.Stack.ToArray();

            // Assert
            Assert.AreEqual(1, forth.Stack.Count);
            Assert.AreEqual("aef", results[0]);

            // Act
            forth.Stack.Clear();
            await forth.Execute("'aaa' 3 *");
            results = forth.Stack.ToArray();

            // Assert
            Assert.AreEqual(1, forth.Stack.Count);
            Assert.AreEqual("aaaaaaaaa", results[0]);
        }
        [Test]
        public async void Test_TokenAction_TDFModeWorks()
        {
            string [] results;
            // Arrange
            var forth = new BasicForth();
            Assert.IsTrue(forth != null);

            // Act
            forth.Stack.Clear();
            await forth.Execute("[aaa]");
            results = forth.Stack.ToArray();

            // Assert
            Assert.AreEqual(1, forth.Stack.Count);
            Assert.AreEqual("aaa", results[0]);

            // Act
            forth.Stack.Clear();
            await forth.Execute("[\n     aaa\n]");
            results = forth.Stack.ToArray();

            // Assert
            Assert.AreEqual(1, forth.Stack.Count);
            Assert.AreEqual("aaa", results[0]);

            // Act
            forth.Stack.Clear();
            results = forth.Tokenize(@"[
          aaa*
          bbb*ccc
]").ToArray();

            // Assert
            Assert.AreEqual(5, results.Length);
            Assert.AreEqual("\"aaa\"", results[0]);
            Assert.AreEqual(".", results[1]);
            Assert.AreEqual("\"bbb\"", results[2]);
            Assert.AreEqual(".", results[3]);
            Assert.AreEqual("\"ccc\"", results[4]);
        }
    }
}
