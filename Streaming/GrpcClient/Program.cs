using Grpc.Core;
using Grpc.Net.Client;
using GrpcClient;
using System.Net.NetworkInformation;


//var channel = GrpcChannel.ForAddress("https://localhost:5001");
//await CallPingReply(new Ping.PingClient(channel));
await ClientStreamingFactory.CallDitto();

//await ClientStreamingFactory.ServerStreamingDemoAsync();
//await ClientStreamingFactory.ClientStreamingDemo();
//await ClientStreamingFactory.BidirectionalStreamingDemo();




