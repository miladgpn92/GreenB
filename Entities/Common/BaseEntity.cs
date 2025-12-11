using Common.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public interface IEntity
    {
    }

    public abstract class BaseEntity<TKey> : IEntity
    {
        public TKey Id { get; set; }
        public BaseEntity()
        {
            CmsLanguage = CmsLanguage.Persian;
        }
        [DataType("tinyint")]
        public CmsLanguage CmsLanguage { get; set; }

        public int? CreatorUserId { get; set; }

        [ForeignKey("CreatorUserId")]
        #nullable enable
        public ApplicationUser? ApplicationUser { get; set; }


        public string CreatorIP { get; set; }
        public DateTime? CreateDate { get; set; }
    }

    public abstract class BaseEntity : BaseEntity<int>
    {
    }

    public abstract class SimpleBaseEntity<TKey> : IEntity
    {
        #nullable disable
        public TKey Id { get; set; }
    }
    public abstract class SimpleBaseEntity : SimpleBaseEntity<int>
    {
    }


    public abstract class BaseWithSeoEntity<Tkey> : IEntity
    {
        public Tkey Id { get; set; }
        public BaseWithSeoEntity()
        {
            CreateDate = DateTime.Now;
            CmsLanguage = CmsLanguage.Persian;
            PublishDate = DateTime.Now; 
        }


        public int? CreatorUserId { get; set; }

        [ForeignKey("CreatorUserId")]
        #nullable enable
        public ApplicationUser? ApplicationUser { get; set; }


        public DateTime? PublishDate { get; set; }


        public string CreatorIP { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? SeoTitle { get; set; }
        public string? SeoDescription { get; set; }
        [DataType("tinyint")]
        public CmsLanguage CmsLanguage { get; set; }
    }
    public abstract class BaseWithSeoEntity : BaseWithSeoEntity<int>
    {

    }
}
