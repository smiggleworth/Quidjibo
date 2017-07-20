# The QuidjiboBuilder

## Overview
All of the components can be wired up from your favorite DI framework, but that can be a bit much. So we have a builder to help simplify the basics. The builder also allows for extension methods to be added by other integrations.

## Anatomy Of the Builder
For the most part the build is a handful of Configure methods that create the basic infrastructure. When using specific integrations such as SQL Server, or Amazon SQS, you want to tell Quidjibo about those things. Typically your use of the builder should be restricted to using the extension methods provided by the specific integration. If you are adding an integration then you most likely will be leveraging those Configure methods while building your custom extensions.

## Building A Server
The builder has a BuildServer() method that will assemble all of the configurations that you need. This method only creates and instance of the QuidjiboServer, you will need to Start(), and Stop() the server yourself. Start and stop can be done manually or if you are using aspnetcore pipeline you can tie into that process, which has a supported integration too.

## Building A Client
The client is how you queue jobs, and schedule work. Using the same builder you used for building your server you can call the BuildClient(). Building the client will create a singleton of your configured client. You can then leverage the QuidjiboClient.Instance to queue, and schedule work. 

But I don't like statics it makes my code hard to test. In that case you can use IQuidjiboClient and inject that into your constructors. Checkout the DI integrations so that the client resolves correctly. 

## Building Multiple Server
Each server runs a single configuration. There may be times when you need to work with multiple types of infrastructures for different parts of your application. This use case is handled by creating two or more builders, configuring them as needed, then starting, and stopping them as needed.

## Building Multiple Clients
Building muliple clients is supported too. However your static is slightly different. Using some generics we can get there but it is a little more work. First we need to create a key to diffentiate each client. 

```C#
public class MyClientKey1 : IQuidjiboClientKey{}
public class MyClientKey2 : IQuidjiboClientKey{}
```

We can use these keys to create distinct clients using the BuildClient<MyClientKey1>() called on our first builder, and on the second builder BuildClient<MyClientKey2>(). Now we can use QuidjiboClient<MyClientKey1>.Instance or QuidjiboClient<MyClientKey2>.Instance. Each one is a distinct configuration. For DI we can inject IQuidjiboClient<MyClientKey1> or IQuidjiboClient<MyClientKey2> into our constructor.

