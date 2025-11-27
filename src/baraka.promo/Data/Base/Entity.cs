using System.ComponentModel.DataAnnotations;

namespace baraka.promo.Data.Base
{
    public class Entity
    {
        protected Entity() { }

        protected Entity(string user)
        {
            CreatedBy = user;
            CreatedTime = DateTime.Now;
            Modified(user);
        }

        [MaxLength(256)]
        public string CreatedBy { get; protected set; }
        public DateTime CreatedTime { get; protected set; }

        [MaxLength(256)]
        public string ModifiedBy { get; protected set; }
        public DateTime ModifiedTime { get; protected set; }

        public bool IsDeleted { get; private set; }
        public void Delete(string user)
        {
            IsDeleted = true;

            Modified(user);
        }

        protected void Modified(string user)
        {
            ModifiedBy = user;
            ModifiedTime = DateTime.Now;
        }
    }
}
