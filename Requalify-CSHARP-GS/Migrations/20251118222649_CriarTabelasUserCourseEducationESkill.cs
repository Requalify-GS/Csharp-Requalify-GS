using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Requalify_CSHARP_GS.Migrations
{
    /// <inheritdoc />
    public partial class CriarTabelasUserCourseEducationESkill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "RM554694");

            migrationBuilder.CreateTable(
                name: "TB_USERS",
                schema: "RM554694",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    SENHA = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    TELEFONE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    DATA_NASCIMENTO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    CARGO_ATUAL = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    AREA_INTERESSE = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_USERS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TB_COURSE",
                schema: "RM554694",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    TITLE = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    DESCRIPTION = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    CATEGORY = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    DIFFICULTY = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    URL = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    USER_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    UPDATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_COURSE", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TB_COURSE_TB_USERS_USER_ID",
                        column: x => x.USER_ID,
                        principalSchema: "RM554694",
                        principalTable: "TB_USERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_EDUCATION",
                schema: "RM554694",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    USER_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DEGREE = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    INSTITUTION = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    COMPLETION_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    CERTIFICATE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_EDUCATION", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TB_EDUCATION_TB_USERS_USER_ID",
                        column: x => x.USER_ID,
                        principalSchema: "RM554694",
                        principalTable: "TB_USERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_SKILL",
                schema: "RM554694",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NAME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    LEVEL = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    CATEGORY = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    PROFICIENCY_PERCENTAGE = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DESCRIPTION = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    USER_ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_SKILL", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TB_SKILL_TB_USERS_USER_ID",
                        column: x => x.USER_ID,
                        principalSchema: "RM554694",
                        principalTable: "TB_USERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_COURSE_USER_ID",
                schema: "RM554694",
                table: "TB_COURSE",
                column: "USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_TB_EDUCATION_USER_ID",
                schema: "RM554694",
                table: "TB_EDUCATION",
                column: "USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_TB_SKILL_USER_ID",
                schema: "RM554694",
                table: "TB_SKILL",
                column: "USER_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_COURSE",
                schema: "RM554694");

            migrationBuilder.DropTable(
                name: "TB_EDUCATION",
                schema: "RM554694");

            migrationBuilder.DropTable(
                name: "TB_SKILL",
                schema: "RM554694");

            migrationBuilder.DropTable(
                name: "TB_USERS",
                schema: "RM554694");
        }
    }
}
