using Microsoft.AspNetCore.SignalR.Client;
using sisae.DTOs;
using System;
using System.Threading.Tasks;

namespace sisae.Services
{
    public class SignalRService
    {
        private readonly string _hubUrl;

        public SignalRService(string hubUrl)
        {
            _hubUrl = hubUrl;
        }

        public async Task SendVisitaUpdateAsync(VisitaDto visitaDto)
        {
            await using var hubConnection = new HubConnectionBuilder()
                .WithUrl(_hubUrl)
                .WithAutomaticReconnect()
                .Build();

            try
            {
                await hubConnection.StartAsync();

                if (hubConnection.State == HubConnectionState.Connected)
                {
                    Console.WriteLine("Conexión establecida al Hub.");
                    await hubConnection.InvokeAsync("SendVisitaUpdate", visitaDto);
                }
                else
                {
                    Console.WriteLine("No se pudo conectar al Hub.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en SignalRService: {ex.Message}");
                throw;
            }
        }

        public async Task SendAccesoProhibidoUpdateAsync(VisitaProhibidosDto accesoProhibidoDto)
        {
            await using var hubConnection = new HubConnectionBuilder()
                .WithUrl(_hubUrl)
                .WithAutomaticReconnect()
                .Build();

            try
            {
                await hubConnection.StartAsync();

                if (hubConnection.State == HubConnectionState.Connected)
                {
                    Console.WriteLine("Conexión establecida al Hub.");
                    await hubConnection.InvokeAsync("SendAccesoProhibidoUpdate", accesoProhibidoDto);
                }
                else
                {
                    Console.WriteLine("No se pudo conectar al Hub.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en SignalRService: {ex.Message}");
                throw;
            }
        }
    }
}
