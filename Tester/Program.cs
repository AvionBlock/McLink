using MCLink.Servers;

var mclink = new McwssMcLinkServer();
mclink.Start(8080);
Console.ReadLine();