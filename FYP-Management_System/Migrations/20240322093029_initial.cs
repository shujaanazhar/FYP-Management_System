using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FYP_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Email);
                });

            migrationBuilder.CreateTable(
                name: "Supervisors",
                columns: table => new
                {
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Domain = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supervisors", x => x.Email);
                    table.ForeignKey(
                        name: "FK_Supervisors_Users_Email",
                        column: x => x.Email,
                        principalTable: "Users",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FYPs",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SupervisorId = table.Column<int>(type: "int", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Domain = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SupervisorEmail = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FYPs", x => x.Name);
                    table.ForeignKey(
                        name: "FK_FYPs_Supervisors_SupervisorEmail",
                        column: x => x.SupervisorEmail,
                        principalTable: "Supervisors",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Batch = table.Column<int>(type: "int", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    FYP_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CGPA = table.Column<float>(type: "real", nullable: false),
                    FYPName = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Email);
                    table.ForeignKey(
                        name: "FK_Students_FYPs_FYPName",
                        column: x => x.FYPName,
                        principalTable: "FYPs",
                        principalColumn: "Name");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FYPs_SupervisorEmail",
                table: "FYPs",
                column: "SupervisorEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Students_FYPName",
                table: "Students",
                column: "FYPName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "FYPs");

            migrationBuilder.DropTable(
                name: "Supervisors");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
