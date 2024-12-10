using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sisae.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Visitados",
                columns: table => new
                {
                    ID_Visitado = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Apellido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cargo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Departamento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visitados", x => x.ID_Visitado);
                });

            migrationBuilder.CreateTable(
                name: "Visitantes",
                columns: table => new
                {
                    ID_Visitante = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Apellido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaVencimientoCarnet = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FotoBiometrica = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Nacionalidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RUT = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visitantes", x => x.ID_Visitante);
                });

            migrationBuilder.CreateTable(
                name: "AccesosProhibidos",
                columns: table => new
                {
                    ID_Acceso_Prohibido = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha_Expiracion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Fecha_Prohibicion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ID_Visitante = table.Column<int>(type: "int", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccesosProhibidos", x => x.ID_Acceso_Prohibido);
                    table.ForeignKey(
                        name: "FK_AccesosProhibidos_Visitantes_ID_Visitante",
                        column: x => x.ID_Visitante,
                        principalTable: "Visitantes",
                        principalColumn: "ID_Visitante",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Visitas",
                columns: table => new
                {
                    ID_Visita = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Comentarios = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fecha_Visita = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Hora_Entrada = table.Column<TimeSpan>(type: "time", nullable: false),
                    Hora_Salida = table.Column<TimeSpan>(type: "time", nullable: true),
                    ID_Visitado = table.Column<int>(type: "int", nullable: false),
                    ID_Visitante = table.Column<int>(type: "int", nullable: false),
                    Motivo_Visita = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visitas", x => x.ID_Visita);
                    table.ForeignKey(
                        name: "FK_Visitas_Visitados_ID_Visitado",
                        column: x => x.ID_Visitado,
                        principalTable: "Visitados",
                        principalColumn: "ID_Visitado",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Visitas_Visitantes_ID_Visitante",
                        column: x => x.ID_Visitante,
                        principalTable: "Visitantes",
                        principalColumn: "ID_Visitante",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccesosProhibidos_ID_Visitante",
                table: "AccesosProhibidos",
                column: "ID_Visitante");

            migrationBuilder.CreateIndex(
                name: "IX_Visitas_ID_Visitado",
                table: "Visitas",
                column: "ID_Visitado");

            migrationBuilder.CreateIndex(
                name: "IX_Visitas_ID_Visitante",
                table: "Visitas",
                column: "ID_Visitante");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccesosProhibidos");

            migrationBuilder.DropTable(
                name: "Visitas");

            migrationBuilder.DropTable(
                name: "Visitados");

            migrationBuilder.DropTable(
                name: "Visitantes");
        }
    }
}
