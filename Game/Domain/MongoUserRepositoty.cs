using System;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoUserRepository : IUserRepository
    {
        private readonly IMongoCollection<UserEntity> userCollection;
        public const string CollectionName = "users";

        public MongoUserRepository(IMongoDatabase database)
        {
            userCollection = database.GetCollection<UserEntity>(CollectionName);
            var options = new CreateIndexOptions {Unique = true};
            userCollection.Indexes.CreateOne("{ Login : 1}", options);
        }

        public UserEntity Insert(UserEntity user)
        {
            userCollection.InsertOne(user);
            return user;
        }

        public UserEntity FindById(Guid id)
        {
            return userCollection
                .Find(user => user.Id == id)
                .FirstOrDefault();
        }

        public UserEntity GetOrCreateByLogin(string login)
        {
            var user = userCollection
                .Find(user => user.Login == login)
                .FirstOrDefault();

            if (user != null)
                return user;

            user = new UserEntity(Guid.NewGuid(), login, null, null, 0, Guid.Empty);
            userCollection.InsertOne(user);

            return user;
        }

        public void Update(UserEntity user)
        {
            userCollection.ReplaceOne(foundUser => foundUser.Id == user.Id, user);
            //TODO: Ищи в документации ReplaceXXX
            //throw new NotImplementedException();
        }

        public void Delete(Guid id)
        {
            userCollection.DeleteOne(user => user.Id == id);
        }

        // Для вывода списка всех пользователей (упорядоченных по логину)
        // страницы нумеруются с единицы
        public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
        {
            var usersCount = userCollection.CountDocuments(user => true);

            return new PageList<UserEntity>(
            userCollection
                    .Find(user => true)
                    .SortBy(user => user.Login)
                    .Skip((pageNumber - 1) * pageSize)
                    .Limit(pageSize)
                    .ToList(), 
                usersCount, 
            pageNumber, 
                pageSize);
            //TODO: Тебе понадобятся SortBy, Skip и Limit
            // throw new NotImplementedException();
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}