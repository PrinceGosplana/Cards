using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreCard.Models
{
    public class DataManager
    {
        private CardsDBEntities _db;
        public DataManager()
        {
            _db = new CardsDBEntities();
        }
        public IQueryable<Card> GetCards()
        {
            return _db.Cards;
        }
        public Card GetCard(int id)
        {
            return _db.Cards.SingleOrDefault(it => it.CardId == id);
        }
        public void SaveCard(Card obj)
        {
            Card old = GetCard(obj.CardId);
            old.Amount = obj.Amount;
            _db.SaveChanges();
        }
        public void Save(Card obj)
        {
            _db.AddToCards(obj);
            _db.SaveChanges();
        }
    }
}