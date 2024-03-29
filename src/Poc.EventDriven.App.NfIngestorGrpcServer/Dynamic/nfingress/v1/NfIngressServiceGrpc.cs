// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: nfingress/v1/nf_ingress_service.proto
// </auto-generated>
#pragma warning disable 0414, 1591
#region Designer generated code

using grpc = global::Grpc.Core;

namespace Poc.EventDriven.Protos.NfIngress.V1 {
  public static partial class NfIngressService
  {
    static readonly string __ServiceName = "poc.eventdriven.nfingress.v1.NfIngressService";

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static void __Helper_SerializeMessage(global::Google.Protobuf.IMessage message, grpc::SerializationContext context)
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (message is global::Google.Protobuf.IBufferMessage)
      {
        context.SetPayloadLength(message.CalculateSize());
        global::Google.Protobuf.MessageExtensions.WriteTo(message, context.GetBufferWriter());
        context.Complete();
        return;
      }
      #endif
      context.Complete(global::Google.Protobuf.MessageExtensions.ToByteArray(message));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static class __Helper_MessageCache<T>
    {
      public static readonly bool IsBufferMessage = global::System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(global::Google.Protobuf.IBufferMessage)).IsAssignableFrom(typeof(T));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static T __Helper_DeserializeMessage<T>(grpc::DeserializationContext context, global::Google.Protobuf.MessageParser<T> parser) where T : global::Google.Protobuf.IMessage<T>
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (__Helper_MessageCache<T>.IsBufferMessage)
      {
        return parser.ParseFrom(context.PayloadAsReadOnlySequence());
      }
      #endif
      return parser.ParseFrom(context.PayloadAsNewBuffer());
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Poc.EventDriven.Protos.NfIngress.V1.NfIngressAddManyRequest> __Marshaller_poc_eventdriven_nfingress_v1_NfIngressAddManyRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Poc.EventDriven.Protos.NfIngress.V1.NfIngressAddManyRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Poc.EventDriven.Protos.NfIngress.V1.NfIngressAddManyResponse> __Marshaller_poc_eventdriven_nfingress_v1_NfIngressAddManyResponse = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Poc.EventDriven.Protos.NfIngress.V1.NfIngressAddManyResponse.Parser));

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::Poc.EventDriven.Protos.NfIngress.V1.NfIngressAddManyRequest, global::Poc.EventDriven.Protos.NfIngress.V1.NfIngressAddManyResponse> __Method_AddMany = new grpc::Method<global::Poc.EventDriven.Protos.NfIngress.V1.NfIngressAddManyRequest, global::Poc.EventDriven.Protos.NfIngress.V1.NfIngressAddManyResponse>(
        grpc::MethodType.ClientStreaming,
        __ServiceName,
        "AddMany",
        __Marshaller_poc_eventdriven_nfingress_v1_NfIngressAddManyRequest,
        __Marshaller_poc_eventdriven_nfingress_v1_NfIngressAddManyResponse);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::Poc.EventDriven.Protos.NfIngress.V1.NfIngressServiceReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of NfIngressService</summary>
    [grpc::BindServiceMethod(typeof(NfIngressService), "BindService")]
    public abstract partial class NfIngressServiceBase
    {
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task<global::Poc.EventDriven.Protos.NfIngress.V1.NfIngressAddManyResponse> AddMany(grpc::IAsyncStreamReader<global::Poc.EventDriven.Protos.NfIngress.V1.NfIngressAddManyRequest> requestStream, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public static grpc::ServerServiceDefinition BindService(NfIngressServiceBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_AddMany, serviceImpl.AddMany).Build();
    }

    /// <summary>Register service method with a service binder with or without implementation. Useful when customizing the  service binding logic.
    /// Note: this method is part of an experimental API that can change or be removed without any prior notice.</summary>
    /// <param name="serviceBinder">Service methods will be bound by calling <c>AddMethod</c> on this object.</param>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public static void BindService(grpc::ServiceBinderBase serviceBinder, NfIngressServiceBase serviceImpl)
    {
      serviceBinder.AddMethod(__Method_AddMany, serviceImpl == null ? null : new grpc::ClientStreamingServerMethod<global::Poc.EventDriven.Protos.NfIngress.V1.NfIngressAddManyRequest, global::Poc.EventDriven.Protos.NfIngress.V1.NfIngressAddManyResponse>(serviceImpl.AddMany));
    }

  }
}
#endregion
