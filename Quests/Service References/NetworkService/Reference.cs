﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.34209
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QuestClient.NetworkService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="NetworkService.IQuestService", CallbackContract=typeof(QuestClient.NetworkService.IQuestServiceCallback), SessionMode=System.ServiceModel.SessionMode.Required)]
    public interface IQuestService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IQuestService/GetQuestKeys", ReplyAction="http://tempuri.org/IQuestService/GetQuestKeysResponse")]
        Common.Key[] GetQuestKeys(int questId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IQuestService/GetQuestKeys", ReplyAction="http://tempuri.org/IQuestService/GetQuestKeysResponse")]
        System.Threading.Tasks.Task<Common.Key[]> GetQuestKeysAsync(int questId);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IQuestService/RegisterQuestClient")]
        void RegisterQuestClient(int questId);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IQuestService/RegisterQuestClient")]
        System.Threading.Tasks.Task RegisterQuestClientAsync(int questId);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IQuestServiceCallback {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IQuestService/SensorTriggered")]
        void SensorTriggered(int sensorId);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IQuestService/StartQuest")]
        void StartQuest();
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IQuestService/StopQuest")]
        void StopQuest();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IQuestService/GetQuestState", ReplyAction="http://tempuri.org/IQuestService/GetQuestStateResponse")]
        Common.QuestState GetQuestState();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IQuestServiceChannel : QuestClient.NetworkService.IQuestService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class QuestServiceClient : System.ServiceModel.DuplexClientBase<QuestClient.NetworkService.IQuestService>, QuestClient.NetworkService.IQuestService {
        
        public QuestServiceClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public QuestServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public QuestServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public QuestServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public QuestServiceClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public Common.Key[] GetQuestKeys(int questId) {
            return base.Channel.GetQuestKeys(questId);
        }
        
        public System.Threading.Tasks.Task<Common.Key[]> GetQuestKeysAsync(int questId) {
            return base.Channel.GetQuestKeysAsync(questId);
        }
        
        public void RegisterQuestClient(int questId) {
            base.Channel.RegisterQuestClient(questId);
        }
        
        public System.Threading.Tasks.Task RegisterQuestClientAsync(int questId) {
            return base.Channel.RegisterQuestClientAsync(questId);
        }
    }
}
