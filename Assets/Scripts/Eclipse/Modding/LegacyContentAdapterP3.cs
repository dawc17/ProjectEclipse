using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
namespace Eclipse.Modding
{
    public sealed partial class LegacyContentAdapter
    {
        private readonly List<AchievCounter> _externalCounters=new List<AchievCounter>();
        public void ApplyP3Content()
        {
            if(_externalCounters.Count!=0) return;
            foreach(var counter in _content.Counters)
            {
                var doc=new XmlDocument(); var node=doc.CreateElement("Counter"); doc.AppendChild(node);
                node.SetAttribute("Name",counter.Id.ToString());
                foreach(var definition in _content.Achievements)
                {
                    if(definition.Counter!=counter.Id) continue;
                    var achievement=doc.CreateElement("Achievement"); node.AppendChild(achievement);
                    achievement.SetAttribute("Name",definition.Id.ToString());
                    achievement.SetAttribute("Description",definition.Description.ToString());
                    achievement.SetAttribute("Icon",definition.Icon.ToString());
                    achievement.SetAttribute("CounterValue",definition.Threshold.ToString(CultureInfo.InvariantCulture));
                    achievement.SetAttribute("Hidden",definition.Hidden?"1":"0");
                }
                var native=new AchievCounter(node);
                GameUtils.HHLEKNNJGMJ.MDNKEAFGAOB.Add(native); _externalCounters.Add(native);
            }
            ModProgressionAccess.Read=id=>{
                RequireCounter(id);
                var user=ListSF.CCDKHLAMKKO()?.KJNPJKEHGLE();
                if(user==null) throw new ModContentException("Profile is unavailable.");
                return user.KJPLIHEMLJL(id.ToString())?.MCIPEJBLIDC()??0;
            };
            ModProgressionAccess.Advance=(id,amount)=>{
                var definition=RequireCounter(id);
                var user=ListSF.CCDKHLAMKKO()?.KJNPJKEHGLE();
                if(user==null) throw new ModContentException("Profile is unavailable.");
                return user.AdvanceExternalCounter(id.ToString(),amount,definition.Maximum);
            };
        }
        private ModCounterDefinition RequireCounter(DefinitionId id)
        {
            foreach(var counter in _content.Counters) if(counter.Id==id) return counter;
            throw new ModContentException("Counter is not active: "+id);
        }
        private void RemoveP3Content()
        {
            ModProgressionAccess.Clear();
            foreach(var counter in _externalCounters) GameUtils.HHLEKNNJGMJ.MDNKEAFGAOB.Remove(counter);
            _externalCounters.Clear();
        }
        private void ApplyP3Localization(string language)
        {
            foreach(var definition in _content.Achievements)
                if(_content.TryGetLocalization(definition.Title,out var title))
                {
                    LocalizationManager.SetExternalString(definition.Id.ToString(),title.GetOrEnglish(language));
                    _localizationKeys.Add(definition.Id.ToString());
                }
        }
    }
}
