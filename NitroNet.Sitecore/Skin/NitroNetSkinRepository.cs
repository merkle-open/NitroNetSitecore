using System;
using NitroNet.ViewEngine;

namespace NitroNet.Sitecore.Skin
{
    public class NitroNetSkinRepository : ISkinRepository
    {
        private readonly IComponentRepository _componentRepository;

        public NitroNetSkinRepository(IComponentRepository componentRepository)
        {
            _componentRepository = componentRepository;
        }

        public bool TryGetSkinDefinition(string id, out ISkinDefinition skinDefinition)
        {
            skinDefinition = null;
            var componentDefinitionTask = _componentRepository.GetComponentDefinitionByIdAsync(id);
            try
            {
                var componentDefinition = componentDefinitionTask.Result;
                if (componentDefinition != null)
                {
                    skinDefinition = new NitroNetSkinDefinition(componentDefinition);
                    return true;
                }
            }
            catch (AggregateException)
            {
            }
            return false;
        }

        public bool TryGetSkinTemplateInfo(string id, string skin, out ITemplateInfo templateInfo)
        {
            templateInfo = null;
            var componentDefinitionTask = _componentRepository.GetComponentDefinitionByIdAsync(id);
            try
            {
                var componentDefinition = componentDefinitionTask.Result;
                if (componentDefinition != null)
                {
                    FileTemplateInfo fileTemplateInfo;
                    if (componentDefinition.Skins.TryGetValue(skin, out fileTemplateInfo))
                    {
                        templateInfo = new NitroNetTemplateInfo(fileTemplateInfo);
                        return true;
                    }
                }
            }
            catch (AggregateException)
            {
            }
            return false;
        }
    }
}