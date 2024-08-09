using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgileKnowledge.Service.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "chat");

            migrationBuilder.EnsureSchema(
                name: "knowledge");

            migrationBuilder.CreateTable(
                name: "Application",
                schema: "chat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Prompt = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    ChatModel = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Temperature = table.Column<double>(type: "numeric(3,2)", nullable: false),
                    MaxResponseToken = table.Column<int>(type: "integer", nullable: false),
                    Template = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    Opener = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleterUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Application", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Base",
                schema: "knowledge",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Icon = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Model = table.Column<string>(type: "text", nullable: true),
                    EmbeddingModel = table.Column<string>(type: "text", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleterUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Base", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileStorages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Path = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    IsCompression = table.Column<bool>(type: "boolean", nullable: false),
                    FullName = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    Type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleterUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileStorages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Account = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Password = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Salt = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Avatar = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    Email = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    IsDisable = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleterUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dialog",
                schema: "chat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    ChatApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleterUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dialog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dialog_Application_ChatApplicationId",
                        column: x => x.ChatApplicationId,
                        principalSchema: "chat",
                        principalTable: "Application",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatApplicationKnowledgeBase",
                columns: table => new
                {
                    ChatApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    KnowledgeBasesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatApplicationKnowledgeBase", x => new { x.ChatApplicationId, x.KnowledgeBasesId });
                    table.ForeignKey(
                        name: "FK_ChatApplicationKnowledgeBase_Application_ChatApplicationId",
                        column: x => x.ChatApplicationId,
                        principalSchema: "chat",
                        principalTable: "Application",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatApplicationKnowledgeBase_Base_KnowledgeBasesId",
                        column: x => x.KnowledgeBasesId,
                        principalSchema: "knowledge",
                        principalTable: "Base",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Details",
                schema: "knowledge",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    MaxTokensPerParagraph = table.Column<int>(type: "integer", nullable: false),
                    MaxTokensPerLine = table.Column<int>(type: "integer", nullable: false),
                    OverlappingTokens = table.Column<int>(type: "integer", nullable: false),
                    TrainingPattern = table.Column<int>(type: "integer", nullable: false),
                    QAPromptTemplate = table.Column<string>(type: "character varying(10240)", maxLength: 10240, nullable: true),
                    FileId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataCount = table.Column<int>(type: "integer", nullable: false),
                    KnowledgeBaseId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleterUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Details_Base_KnowledgeBaseId",
                        column: x => x.KnowledgeBaseId,
                        principalSchema: "knowledge",
                        principalTable: "Base",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Details_FileStorages_FileId",
                        column: x => x.FileId,
                        principalTable: "FileStorages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DialogHistory",
                schema: "chat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    TokenConsumption = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    ChatDialogId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleterUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DialogHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DialogHistory_Dialog_ChatDialogId",
                        column: x => x.ChatDialogId,
                        principalSchema: "chat",
                        principalTable: "Dialog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatApplicationKnowledgeBase_KnowledgeBasesId",
                table: "ChatApplicationKnowledgeBase",
                column: "KnowledgeBasesId");

            migrationBuilder.CreateIndex(
                name: "IX_Details_FileId",
                schema: "knowledge",
                table: "Details",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_Details_KnowledgeBaseId",
                schema: "knowledge",
                table: "Details",
                column: "KnowledgeBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Dialog_ChatApplicationId",
                schema: "chat",
                table: "Dialog",
                column: "ChatApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_DialogHistory_ChatDialogId",
                schema: "chat",
                table: "DialogHistory",
                column: "ChatDialogId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatApplicationKnowledgeBase");

            migrationBuilder.DropTable(
                name: "Details",
                schema: "knowledge");

            migrationBuilder.DropTable(
                name: "DialogHistory",
                schema: "chat");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Base",
                schema: "knowledge");

            migrationBuilder.DropTable(
                name: "FileStorages");

            migrationBuilder.DropTable(
                name: "Dialog",
                schema: "chat");

            migrationBuilder.DropTable(
                name: "Application",
                schema: "chat");
        }
    }
}
