using Microsoft.EntityFrameworkCore.Migrations;

namespace ChatApi.Migrations
{
    public partial class NombresApellidosUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.AddColumn<string>(
                name: "Apellidos",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nombres",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

           
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
