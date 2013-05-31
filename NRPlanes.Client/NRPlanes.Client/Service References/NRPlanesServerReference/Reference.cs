﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NRPlanes.Client.NRPlanesServerReference {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="NRPlanesServerReference.IGameService")]
    public interface IGameService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGameService/Join", ReplyAction="http://tempuri.org/IGameService/JoinResponse")]
        NRPlanes.ServerData.OperationResults.JoinResult Join();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGameService/CommitObjects", ReplyAction="http://tempuri.org/IGameService/CommitObjectsResponse")]
        NRPlanes.ServerData.OperationResults.CommitResult CommitObjects(System.Guid playerGuid, NRPlanes.Core.Common.GameObject[] objects);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGameService/GetNewObjects", ReplyAction="http://tempuri.org/IGameService/GetNewObjectsResponse")]
        NRPlanes.ServerData.OperationResults.GetNewObjectsResult GetNewObjects(System.Guid playerGuid, int minId);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IGameService/UpdatePlane")]
        void UpdatePlane(System.Guid playerGuid, NRPlanes.ServerData.MutableInformations.PlaneMutableInformation info);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGameService/GetPlanesInfo", ReplyAction="http://tempuri.org/IGameService/GetPlanesInfoResponse")]
        NRPlanes.ServerData.MutableInformations.PlaneMutableInformation[] GetPlanesInfo(System.Guid playerGuid);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IGameServiceChannel : NRPlanes.Client.NRPlanesServerReference.IGameService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GameServiceClient : System.ServiceModel.ClientBase<NRPlanes.Client.NRPlanesServerReference.IGameService>, NRPlanes.Client.NRPlanesServerReference.IGameService {
        
        public GameServiceClient() {
        }
        
        public GameServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public GameServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public GameServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public GameServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public NRPlanes.ServerData.OperationResults.JoinResult Join() {
            return base.Channel.Join();
        }
        
        public NRPlanes.ServerData.OperationResults.CommitResult CommitObjects(System.Guid playerGuid, NRPlanes.Core.Common.GameObject[] objects) {
            return base.Channel.CommitObjects(playerGuid, objects);
        }
        
        public NRPlanes.ServerData.OperationResults.GetNewObjectsResult GetNewObjects(System.Guid playerGuid, int minId) {
            return base.Channel.GetNewObjects(playerGuid, minId);
        }
        
        public void UpdatePlane(System.Guid playerGuid, NRPlanes.ServerData.MutableInformations.PlaneMutableInformation info) {
            base.Channel.UpdatePlane(playerGuid, info);
        }
        
        public NRPlanes.ServerData.MutableInformations.PlaneMutableInformation[] GetPlanesInfo(System.Guid playerGuid) {
            return base.Channel.GetPlanesInfo(playerGuid);
        }
    }
}