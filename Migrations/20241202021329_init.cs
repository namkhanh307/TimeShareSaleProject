using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeShareProject.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    password = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    role = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Account__3214EC27CAA521F4", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Block",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    startDay = table.Column<int>(type: "int", nullable: true),
                    startMonth = table.Column<int>(type: "int", nullable: true),
                    endDay = table.Column<int>(type: "int", nullable: true),
                    endMonth = table.Column<int>(type: "int", nullable: true),
                    blockNumber = table.Column<int>(type: "int", nullable: true),
                    proportion = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Block", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Contact",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    status = table.Column<bool>(type: "bit", nullable: false),
                    phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contact", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    shortName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    totalUnit = table.Column<int>(type: "int", nullable: true),
                    image1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    image2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    image3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    generalDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    detailDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true),
                    area = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    sex = table.Column<bool>(type: "bit", nullable: true),
                    dateOfBirth = table.Column<DateTime>(type: "datetime", nullable: true),
                    phoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IDFrontImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IDBackImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    accountID = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.ID);
                    table.ForeignKey(
                        name: "FK_User_Account",
                        column: x => x.accountID,
                        principalTable: "Account",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Property",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    saleDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    unitPrice = table.Column<double>(type: "float", nullable: true),
                    projectID = table.Column<int>(type: "int", nullable: false),
                    beds = table.Column<int>(type: "int", nullable: true),
                    occupancy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    size = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bathroom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    views = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    uniqueFeature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    viewImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    frontImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    insideImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sideImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacationList", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Vacation_Project",
                        column: x => x.projectID,
                        principalTable: "Project",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Rate",
                columns: table => new
                {
                    projectID = table.Column<int>(type: "int", nullable: false),
                    userID = table.Column<int>(type: "int", nullable: false),
                    detailRate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    starRate = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Rate__70B22D9023FDCE83", x => new { x.projectID, x.userID });
                    table.ForeignKey(
                        name: "FK_Rate_Project",
                        column: x => x.projectID,
                        principalTable: "Project",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK__Rate__userID__18EBB532",
                        column: x => x.userID,
                        principalTable: "User",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Reservation",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    propertyID = table.Column<int>(type: "int", nullable: false),
                    userID = table.Column<int>(type: "int", nullable: false),
                    registerDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    yearQuantity = table.Column<int>(type: "int", nullable: true),
                    type = table.Column<int>(type: "int", nullable: true),
                    blockID = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: true),
                    order = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacationRegistration", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Reservation_Block",
                        column: x => x.blockID,
                        principalTable: "Block",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Reservation_Property",
                        column: x => x.propertyID,
                        principalTable: "Property",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservation_User",
                        column: x => x.userID,
                        principalTable: "User",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Transaction",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    date = table.Column<DateTime>(type: "datetime", nullable: true),
                    amount = table.Column<double>(type: "float", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true),
                    transactionCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    reservationID = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<int>(type: "int", nullable: true),
                    deadlineDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaction", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Transaction_Reservation",
                        column: x => x.reservationID,
                        principalTable: "Reservation",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "New",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userID = table.Column<int>(type: "int", nullable: true),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    date = table.Column<DateTime>(type: "datetime", nullable: true),
                    type = table.Column<int>(type: "int", nullable: true),
                    transactionID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_New", x => x.ID);
                    table.ForeignKey(
                        name: "FK_New_Transaction",
                        column: x => x.transactionID,
                        principalTable: "Transaction",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_New_User",
                        column: x => x.userID,
                        principalTable: "User",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_New_transactionID",
                table: "New",
                column: "transactionID");

            migrationBuilder.CreateIndex(
                name: "IX_New_userID",
                table: "New",
                column: "userID");

            migrationBuilder.CreateIndex(
                name: "IX_Property_projectID",
                table: "Property",
                column: "projectID");

            migrationBuilder.CreateIndex(
                name: "IX_Rate_userID",
                table: "Rate",
                column: "userID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_blockID",
                table: "Reservation",
                column: "blockID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_propertyID",
                table: "Reservation",
                column: "propertyID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_userID",
                table: "Reservation",
                column: "userID");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_reservationID",
                table: "Transaction",
                column: "reservationID");

            migrationBuilder.CreateIndex(
                name: "IX_User_accountID",
                table: "User",
                column: "accountID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contact");

            migrationBuilder.DropTable(
                name: "New");

            migrationBuilder.DropTable(
                name: "Rate");

            migrationBuilder.DropTable(
                name: "Transaction");

            migrationBuilder.DropTable(
                name: "Reservation");

            migrationBuilder.DropTable(
                name: "Block");

            migrationBuilder.DropTable(
                name: "Property");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.DropTable(
                name: "Account");
        }
    }
}
