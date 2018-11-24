using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using CodeKicker.BBCode.SyntaxTree;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeKicker.BBCode.Tests.Unit
{
    [TestClass]
    public partial class SyntaxTreeVisitorTest
    {
        [TestMethod]
        public void DefaultVisitorModifiesNothing()
        {
            var tree = BBCodeTestUtil.GetAnyTree();
            var tree2 = new SyntaxTreeVisitor().Visit(tree);
            Assert.IsTrue(ReferenceEquals(tree, tree2));
        }

        [TestMethod]
        public void IdentityModifiedTreesAreEqual()
        {
            var tree = BBCodeTestUtil.GetAnyTree();
            var tree2 = new IdentitiyModificationSyntaxTreeVisitor().Visit(tree);
            Assert.IsTrue(tree == tree2);
        }

        [TestMethod]
        public void TextModifiedTreesAreNotEqual()
        {
            var tree = BBCodeTestUtil.GetAnyTree();
            var tree2 = new TextModificationSyntaxTreeVisitor().Visit(tree);
            Assert.IsTrue(tree != tree2);
        }

        class IdentitiyModificationSyntaxTreeVisitor : SyntaxTreeVisitor
        {
            protected internal new SyntaxTreeNode Visit(TextNode node)
            {
                if (!(DateTime.Now.Millisecond % 2 == 0)) return base.Visit(node);

                return new TextNode(node.Text, node.HtmlTemplate);
            }
            protected internal new SyntaxTreeNode Visit(SequenceNode node)
            {
                var baseResult = base.Visit(node);
                if (!(DateTime.Now.Millisecond % 2 == 0)) return baseResult;
                return baseResult.SetSubNodes(baseResult.SubNodes.ToList());
            }
            protected internal new SyntaxTreeNode Visit(TagNode node)
            {
                var baseResult = base.Visit(node);
                if (!(DateTime.Now.Millisecond % 2 == 0)) return baseResult;
                return baseResult.SetSubNodes(baseResult.SubNodes.ToList());
            }
        }

        class TextModificationSyntaxTreeVisitor : SyntaxTreeVisitor
        {
            protected internal new SyntaxTreeNode Visit(TextNode node)
            {
                return new TextNode(node.Text + "x", node.HtmlTemplate);
            }
            protected internal new SyntaxTreeNode Visit(SequenceNode node)
            {
                var baseResult = base.Visit(node);
                return baseResult.SetSubNodes(baseResult.SubNodes.Concat(new[] { new TextNode("y") }));
            }
            protected internal new SyntaxTreeNode Visit(TagNode node)
            {
                var baseResult = base.Visit(node);
                return baseResult.SetSubNodes(baseResult.SubNodes.Concat(new[] { new TextNode("z") }));
            }
        }
    }
}
