﻿namespace BlueDotBrigade.DatenLokator.TestTools
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	[TestClass]
	public class AsJsonShould
	{
		public class TestData
		{
			public int Id { get; set; }
			public string Name { get; set; }
		}

		[TestMethod]
		public void FromJson_ByConvention_ReturnsLocalFileContent()
		{
			// Arrange
			var expectedData = new TestData { Id = 1, Name = "Sample" };

			// Act
			var actualData = new Daten().FromJson<TestData>();


			// Assert
			Assert.AreEqual(expectedData.Id, actualData.Id);
			Assert.AreEqual(expectedData.Name, actualData.Name);
		}
	}
}
