using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomReservations.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Reservations_ReservationId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_ReservationId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ReservationId",
                table: "Transactions");

            migrationBuilder.CreateTable(
                name: "ReservationTransactions",
                columns: table => new
                {
                    ReservationId = table.Column<int>(type: "INTEGER", nullable: false),
                    TransactionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationTransactions", x => new { x.ReservationId, x.TransactionId });
                    table.ForeignKey(
                        name: "FK_ReservationTransactions_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReservationTransactions_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReservationTransactions_TransactionId",
                table: "ReservationTransactions",
                column: "TransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReservationTransactions");

            migrationBuilder.AddColumn<int>(
                name: "ReservationId",
                table: "Transactions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ReservationId",
                table: "Transactions",
                column: "ReservationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Reservations_ReservationId",
                table: "Transactions",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "Id");
        }
    }
}