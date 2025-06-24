using MCLink.Servers.Mcwss;

var mclink = new McwssMcLinkServer();
mclink.Start(8080);
Console.ReadLine();