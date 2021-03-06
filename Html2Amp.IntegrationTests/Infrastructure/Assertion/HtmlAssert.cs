﻿using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
	public static class HtmlAssert
	{
		public static void AreEqual(string expectedHtml, string actualHtml)
		{
			var htmlParser = new HtmlParser();

			var expected = htmlParser.Parse(expectedHtml);
			var actual = htmlParser.Parse(actualHtml);

			AreEqualCore(expected.Body, actual.Body);
		}

		private static void AreEqualCore(IElement expected, IElement actual)
		{
			if (expected == null || actual == null)
			{
				return;
			}

			Assert.AreEqual(expected.TagName, actual.TagName, GenerateMessage(expected, actual));

			CheckAttributesAreEqual(expected, actual);

			foreach (var exprectedChild in expected.Children)
			{
				foreach (var actualChild in actual.Children)
				{
					AreEqualCore(exprectedChild, actualChild);
				}
			}
		}

		private static void CheckAttributesAreEqual(IElement expected, IElement actual)
		{
			Assert.AreEqual(expected.Attributes.Length, actual.Attributes.Length, string.Format(
					"One of the elements has more attributes! {0}",
					GenerateMessage(expected, actual)));

			var attributesEqual = expected.Attributes.All(x => actual.Attributes.Contains(x, new AttributeEqualityComparer()));

			if (!attributesEqual)
			{
				Assert.Fail(string.Format("The attributes for elements does not match! {0}", GenerateMessage(expected, actual)));
			}
		}

		private static string GenerateMessage(IElement expected, IElement actual)
		{
			return string.Format("{0}Expected HTML:{0}{1}{0}Actual HTML:{0}{2}", Environment.NewLine, expected.OuterHtml, actual.OuterHtml);
		}
	}

	class AttributeEqualityComparer : IEqualityComparer<IAttr>
	{
		public bool Equals(IAttr expected, IAttr actual)
		{
			return expected.Name == actual.Name && expected.Value == actual.Value;
		}

		public int GetHashCode(IAttr obj)
		{
			return obj.GetHashCode();
		}
	}
}