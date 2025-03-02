using FinanceControl.Application.Extensions.Utils.Cryptography;
using FinanceControl.Application.Services.User.Repository;
using FinanceControl.Domain.Entities;
using MongoDB.Driver;
using Moq;

namespace FinanceControl.Test
{
    public class UserTest
    {
        private readonly Mock<IUserRepository> _repository;
        private readonly List<UserModel> _mockUsers;

        public UserTest()
        {
            _repository = new Mock<IUserRepository>();
            _mockUsers = [];

            _repository.Setup(r => r.InsertOneAsync(It.IsAny<UserModel>()))
                       .Callback((UserModel model) => _mockUsers.Add(model))
                       .Returns(Task.CompletedTask);
        }

        [Theory]
        [InlineData("Teste", "teste@teste.com", "Teste.1", "Teste.1")]
        public async Task RegisterSucess(string name, string email, string password, string confirmPassword)
        {
            var model = new UserModel(name, email, password.EncryptPassword());
            await _repository.Object.InsertOneAsync(model);

            var result = _mockUsers.FirstOrDefault(x => x.Email == email);

            Assert.Equal(password, confirmPassword);
            Assert.NotNull(result);
            Assert.Contains(_mockUsers, x => x.UserId != Guid.Empty);
        }

        [Theory]
        [InlineData("Teste", "teste@teste.com", "Teste.1", "Teste.2")]
        public async Task RegisterFail(string name, string email, string password, string confirmPassword)
        {
            var model = new UserModel(name, email, password.EncryptPassword());
            Assert.NotEqual(password, confirmPassword);
            await _repository.Object.InsertOneAsync(model);

            var result = _mockUsers.FirstOrDefault(x => x.Email == email);

            Assert.NotNull(result);
            Assert.Contains(_mockUsers, x => x.UserId != Guid.Empty);
        }
    }
}