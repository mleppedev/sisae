document.addEventListener("DOMContentLoaded", function () {
    const ctx = document.getElementById('realtimeChart').getContext('2d');
    
    const chart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: [], // Etiquetas de tiempo
            datasets: [{
                label: 'Ingresos',
                backgroundColor: 'rgb(75, 192, 192)',
                borderColor: 'rgb(75, 192, 192)',
                data: [] // Los datos se actualizarán aquí
            }]
        },
        options: {
            scales: {
                x: {
                    type: 'realtime',
                    realtime: {
                        onRefresh: function() {
                            // Lógica de actualización
                        }
                    }
                }
            }
        }
    });

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5278/Dashboard")
        .build();

        connection.on("ReceiveVisita", (message) => {
            const newData = JSON.parse(message);
            chart.data.labels.push(newData.fecha);
            chart.data.datasets[0].data.push(1); // Por ejemplo, para contar una visita
            chart.update('quiet');
        });

    connection.start().catch(err => console.error(err.toString()));
});
