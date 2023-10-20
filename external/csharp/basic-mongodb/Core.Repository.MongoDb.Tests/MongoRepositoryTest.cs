using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Core.Repository.MongoDb.Tests
{
	[TestFixture]
	public class MongoRepositoryTest
	{
		private IRepository<Product> _prodRepository;

		public MongoRepositoryTest()
		{
			var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MongoDb.ConnectionString"].ToString();
			this._prodRepository = new MongoRepository<Product>(connectionString);
		}
		[Test]
		public async Task Get_Valid_Entity_Success()
		{
			var product = new Product() {
				Name = "Product" + DateTime.UtcNow.ToString()
			};

			var insertedProduct = await this._prodRepository.Insert(product);
			var productResult = await this._prodRepository.Get(insertedProduct.Id);

			Assert.AreEqual(productResult.Id, insertedProduct.Id);
			Assert.AreEqual(productResult.Name, product.Name);
		}

		[Test]
		public async Task Get_Invalid_Entity_Success()
		{
			var productResult = await this._prodRepository.Get("SomeInvalidId");
			Assert.IsNull(productResult);
		}

		[Test]
		// [TestCategory("Integration")]
        public async Task Get_Null_Entity_Success()
		{
			var productResult = await this._prodRepository.Get(null);
			Assert.IsNull(productResult);
		}

		[Test]
		public async Task Delete_Valid_Entity_Success()
		{
			var product = new Product() {
				Name = "Product" + DateTime.UtcNow.ToString()
			};

			var insertedProduct = await this._prodRepository.Insert(product);
			var getProduct = await this._prodRepository.Get(insertedProduct.Id);

			Assert.IsNotNull(getProduct);
			Assert.AreEqual(getProduct.Id, insertedProduct.Id);

			var deletedProduct = await this._prodRepository.Delete(getProduct.Id);

			Assert.IsNotNull(deletedProduct);
			Assert.AreEqual(deletedProduct.Id, getProduct.Id);
		}

		[Test]
		public async Task Delete_Invalid_Entity_Success()
		{
			var deletedProduct = await this._prodRepository.Delete("InvalidEntityId");
			Assert.IsNull(deletedProduct);
		}

		[Test]
		public async Task Delete_Null_Entity_Success()
		{
			var deletedProduct = await this._prodRepository.Delete(null);
			Assert.IsNull(deletedProduct);
		}

		[Test]
		public async Task Insert_Empty_Entity_Success()
		{
			var productResult = await this._prodRepository.Insert(new Product());
			Assert.IsNotNull(productResult);
			Assert.IsNotNull(productResult.Id);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public async Task Insert_Null_Entity_Exception()
		{
			await this._prodRepository.Insert(null);
		}

		[Test]
		[ExpectedException(typeof(EntityDuplicateException))]
		public async Task Insert_Duplicated_Entity_Exception()
		{
			var prodId = "DuplicatedId" + DateTime.UtcNow.ToString();
			var product = new Product() {
				Id = prodId
			};

			await this._prodRepository.Insert(product);
			await this._prodRepository.Insert(product);
		}

		[Test]
		public async Task Update_Valid_Entity_Success()
		{
			var productName = "Product" + DateTime.UtcNow.ToString();
			var updatedProductName = "Updated Name";

			var product = new Product() {
				Name = productName
			};

			var insertedProduct = await this._prodRepository.Insert(product);
			var productResult = await this._prodRepository.Get(insertedProduct.Id);
			productResult.Name = updatedProductName;

			var updatedProduct = await this._prodRepository.Update(productResult);

			Assert.AreEqual(updatedProduct.Name, updatedProductName);
			Assert.IsTrue(updatedProduct.Version > insertedProduct.Version);
		}

		[Test]
		[ExpectedException(typeof(EntityConflictException))]
		public async Task Update_Version_Conflit_Entity_Exception()
		{
			var productName = "Product" + DateTime.UtcNow.ToString();

			var product = new Product() {
				Name = productName
			};

			var insertedProduct = await this._prodRepository.Insert(product);
			var productResult1 = await this._prodRepository.Get(insertedProduct.Id);
			var productResult2 = await this._prodRepository.Get(insertedProduct.Id);

			var updatedProduct1 = await this._prodRepository.Update(productResult1);
			var updatedProduct2 = await this._prodRepository.Update(productResult2);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public async Task Update_Null_Entity_Exception()
		{
			await this._prodRepository.Update(null); 
		}
		// NOTE:  fails to compile under SharpDevelop
		/*
      	var ex = Assert.Throws<ArgumentNullException>(() => {
        	                                              	await this._prodRepository.Update(null); });

 */
		
	}
}
