using Azure.AI.OpenAI;
using Azure.Identity;
using infrastructure;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.DevUI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using OpenAI.Chat;
using shipment.agents;
using shipment.agents.Group;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAgents(builder.Configuration);
builder.Services.AddMCPClientFactory();
builder.Services.AddAzureTokenClient(builder.Configuration);
builder.Services.AddAgents();

builder.Services.AddOpenAIResponses();
builder.Services.AddOpenAIConversations();

builder.AddWorkflow("multi", (sp, key) =>
{
    var groupagent = sp.GetRequiredService<IGroupAgent>();
    return groupagent.Create().GetAwaiter().GetResult();
}).AddAsAIAgent();


var app = builder.Build();


app.MapOpenAIResponses();
app.MapOpenAIConversations();

if (builder.Environment.IsDevelopment())
{
    app.MapDevUI();
}
app.Run();

