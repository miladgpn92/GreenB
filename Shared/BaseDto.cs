using AutoMapper;
using Entities;
using System.ComponentModel.DataAnnotations;
 
using Common.Enums;
using ResourceLibrary.Resources.SharedModels;
using Common.Utilities;
using ResourceLibrary.Resources.ErrorMsg;
using SharedModels.CustomMapping;

namespace SharedModels.Api
{
    public abstract class BaseDto<TDto, TEntity, TKey> : IHaveCustomMapping
        where TDto : class, new()
        where TEntity : BaseEntity<TKey>, new()
    {

        public BaseDto()
        {
            CmsLanguage = CmsEx.GetCurrentLanguage();
        }

        [Display(Name = "ردیف")]
        [Required(AllowEmptyStrings = true)]
        public TKey Id { get; set; }


        [Display(Name = "CmsLanguage", ResourceType = typeof(SharedModelsRes))]
        public CmsLanguage CmsLanguage { get; set; }

        public TEntity ToEntity(IMapper mapper)
        {
            return mapper.Map<TEntity>(CastToDerivedClass(mapper, this));
        }

        public TEntity ToEntity(IMapper mapper, TEntity entity)
        {
            return mapper.Map(CastToDerivedClass(mapper, this), entity);
        }

        public static TDto FromEntity(IMapper mapper, TEntity model)
        {
            return mapper.Map<TDto>(model);
        }

        protected TDto CastToDerivedClass(IMapper mapper, BaseDto<TDto, TEntity, TKey> baseInstance)
        {
            return mapper.Map<TDto>(baseInstance);
        }

        public void CreateMappings(Profile profile)
        {
            var mappingExpression = profile.CreateMap<TDto, TEntity>();

            var dtoType = typeof(TDto);
            var entityType = typeof(TEntity);
            //Ignore any property of source (like Post.Author) that dose not contains in destination 
            foreach (var property in entityType.GetProperties())
            {
                if (dtoType.GetProperty(property.Name) == null)
                    mappingExpression.ForMember(property.Name, opt => opt.Ignore());
            }

            CustomMappings(mappingExpression.ReverseMap());
        }

        public virtual void CustomMappings(IMappingExpression<TEntity, TDto> mapping)
        {
        }
    }

    public abstract class BaseDto<TDto, TEntity> : BaseDto<TDto, TEntity, int>
        where TDto : class, new()
        where TEntity : BaseEntity<int>, new()
    {

    }



    public abstract class SimpleBaseDto<TDto, TEntity, TKey> : IHaveCustomMapping
    where TDto : class, new()
    where TEntity : SimpleBaseEntity<TKey>, new()
    {
        [Display(Name = "ردیف")]
        [Required(AllowEmptyStrings = true)]
        public TKey Id { get; set; }

        public TEntity ToEntity(IMapper mapper)
        {
            return mapper.Map<TEntity>(CastToDerivedClass(mapper, this));
        }

        public TEntity ToEntity(IMapper mapper, TEntity entity)
        {
            return mapper.Map(CastToDerivedClass(mapper, this), entity);
        }

        public static TDto FromEntity(IMapper mapper, TEntity model)
        {
            return mapper.Map<TDto>(model);
        }

        protected TDto CastToDerivedClass(IMapper mapper, SimpleBaseDto<TDto, TEntity, TKey> baseInstance)
        {
            return mapper.Map<TDto>(baseInstance);
        }

        public void CreateMappings(Profile profile)
        {
            var mappingExpression = profile.CreateMap<TDto, TEntity>();
            CustomMappings(mappingExpression.ReverseMap());
        }

        public virtual void CustomMappings(IMappingExpression<TEntity, TDto> mapping)
        {
        }
    }

    public abstract class SimpleBaseDto<TDto, TEntity> : SimpleBaseDto<TDto, TEntity, int>
        where TDto : class, new()
        where TEntity : SimpleBaseEntity<int>, new()
    {

    }




    public abstract class BaseWithSeoDto<TDto, TEntity, TKey> : IHaveCustomMapping
    where TDto : class, new()
    where TEntity : BaseWithSeoEntity<TKey>, new()
    {
        public BaseWithSeoDto()
        {
            CmsLanguage = CmsEx.GetCurrentLanguage();
        }
        [Display(Name = "ردیف")]
        [Required(AllowEmptyStrings = true)]
        public TKey Id { get; set; }

        [Display(Name = "SeoTitle", ResourceType = typeof(SharedModelsRes))]
        [MaxLength(150, ErrorMessageResourceName = "MaxLenMsg", ErrorMessageResourceType = typeof(ErrorMsg))]
        public string SeoTitle { get; set; }

        [Display(Name = "SeoDescription", ResourceType = typeof(SharedModelsRes))]
        public string SeoDescription { get; set; }

        [Display(Name = "CmsLanguage", ResourceType = typeof(SharedModelsRes))]
        public CmsLanguage CmsLanguage { get; set; }


        public TEntity ToEntity(IMapper mapper)
        {
            return mapper.Map<TEntity>(CastToDerivedClass(mapper, this));
        }

        public TEntity ToEntity(IMapper mapper, TEntity entity)
        {
            return mapper.Map(CastToDerivedClass(mapper, this), entity);
        }

        public static TDto FromEntity(IMapper mapper, TEntity model)
        {
            return mapper.Map<TDto>(model);
        }

        protected TDto CastToDerivedClass(IMapper mapper, BaseWithSeoDto<TDto, TEntity, TKey> baseInstance)
        {
            return mapper.Map<TDto>(baseInstance);
        }

        public void CreateMappings(Profile profile)
        {
            var mappingExpression = profile.CreateMap<TDto, TEntity>();

            var dtoType = typeof(TDto);
            var entityType = typeof(TEntity);
            //Ignore any property of source (like Post.Author) that dose not contains in destination 
            foreach (var property in entityType.GetProperties())
            {
                if (dtoType.GetProperty(property.Name) == null)
                    mappingExpression.ForMember(property.Name, opt => opt.Ignore());
            }

            CustomMappings(mappingExpression.ReverseMap());
        }

        public virtual void CustomMappings(IMappingExpression<TEntity, TDto> mapping)
        {
        }
    }

    public abstract class BaseWithSeoDto<TDto, TEntity> : BaseWithSeoDto<TDto, TEntity, int>
        where TDto : class, new()
        where TEntity : BaseWithSeoEntity<int>, new()
    {

    }





}
