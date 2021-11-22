using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Game.Domain
{
    // TODO Сделать по аналогии с MongoUserRepository
    public class MongoGameRepository : IGameRepository
    {
        public const string CollectionName = "games";
        private readonly IMongoCollection<GameEntity> gameCollection;

        public MongoGameRepository(IMongoDatabase db)
        {
            gameCollection = db.GetCollection<GameEntity>(CollectionName);
        }

        private GameEntity FindByFilter(FilterDefinition<GameEntity> filter)
        {
            using (var cursor = gameCollection.Find(filter).ToCursor())
            {
                while (cursor.MoveNext())
                {
                    foreach (var doc in cursor.Current)
                        return doc;
                }
            }

            return null;
        }
        public GameEntity Insert(GameEntity game)
        {
            gameCollection.InsertOne(game);
            return game;
        }

        public GameEntity FindById(Guid gameId)
        {
            var filter = new BsonDocument("_id", gameId);
            return FindByFilter(filter);
        }

        public void Update(GameEntity game)
        {
            gameCollection.ReplaceOne(foundGame => foundGame.Id == game.Id, game);
        }

        // Возвращает не более чем limit игр со статусом GameStatus.WaitingToStart
        public IList<GameEntity> FindWaitingToStart(int limit)
        {
            return gameCollection
                .Find(game => game.Status == GameStatus.WaitingToStart)
                .Limit(limit)
                .ToList();
            //TODO: Используй Find и Limit
            throw new NotImplementedException();
        }

        // Обновляет игру, если она находится в статусе GameStatus.WaitingToStart
        public bool TryUpdateWaitingToStart(GameEntity game)
        {
            var result = gameCollection.ReplaceOne(game => game.Status == GameStatus.WaitingToStart, game);

            if (result.IsAcknowledged)
                return result.ModifiedCount > 0;
            else
                return false;
            //TODO: Для проверки успешности используй IsAcknowledged и ModifiedCount из результата
            throw new NotImplementedException();
        }
    }
}