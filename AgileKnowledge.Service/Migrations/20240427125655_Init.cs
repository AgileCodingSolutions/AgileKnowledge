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
            migrationBuilder.CreateTable(
                name: "knowledge-chat-application",
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
                    table.PrimaryKey("PK_knowledge-chat-application", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "knowledge-file-storages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Path = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    IsCompression = table.Column<bool>(type: "boolean", nullable: false),
                    FullName = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
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
                    table.PrimaryKey("PK_knowledge-file-storages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "knowledge-knowledge-base",
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
                    table.PrimaryKey("PK_knowledge-knowledge-base", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "knowledge-users",
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
                    table.PrimaryKey("PK_knowledge-users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "knowledge-chat-dialog",
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
                    table.PrimaryKey("PK_knowledge-chat-dialog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_knowledge-chat-dialog_knowledge-chat-application_ChatApplic~",
                        column: x => x.ChatApplicationId,
                        principalTable: "knowledge-chat-application",
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
                        name: "FK_ChatApplicationKnowledgeBase_knowledge-chat-application_Cha~",
                        column: x => x.ChatApplicationId,
                        principalTable: "knowledge-chat-application",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatApplicationKnowledgeBase_knowledge-knowledge-base_Knowl~",
                        column: x => x.KnowledgeBasesId,
                        principalTable: "knowledge-knowledge-base",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "knowledge-knowledge-base-details",
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
                    table.PrimaryKey("PK_knowledge-knowledge-base-details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_knowledge-knowledge-base-details_knowledge-file-storages_Fi~",
                        column: x => x.FileId,
                        principalTable: "knowledge-file-storages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_knowledge-knowledge-base-details_knowledge-knowledge-base_K~",
                        column: x => x.KnowledgeBaseId,
                        principalTable: "knowledge-knowledge-base",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "knowledge-chat-dialog-history",
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
                    table.PrimaryKey("PK_knowledge-chat-dialog-history", x => x.Id);
                    table.ForeignKey(
                        name: "FK_knowledge-chat-dialog-history_knowledge-chat-dialog_ChatDia~",
                        column: x => x.ChatDialogId,
                        principalTable: "knowledge-chat-dialog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatApplicationKnowledgeBase_KnowledgeBasesId",
                table: "ChatApplicationKnowledgeBase",
                column: "KnowledgeBasesId");

            migrationBuilder.CreateIndex(
                name: "IX_knowledge-chat-dialog_ChatApplicationId",
                table: "knowledge-chat-dialog",
                column: "ChatApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_knowledge-chat-dialog-history_ChatDialogId",
                table: "knowledge-chat-dialog-history",
                column: "ChatDialogId");

            migrationBuilder.CreateIndex(
                name: "IX_knowledge-knowledge-base-details_FileId",
                table: "knowledge-knowledge-base-details",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_knowledge-knowledge-base-details_KnowledgeBaseId",
                table: "knowledge-knowledge-base-details",
                column: "KnowledgeBaseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatApplicationKnowledgeBase");

            migrationBuilder.DropTable(
                name: "knowledge-chat-dialog-history");

            migrationBuilder.DropTable(
                name: "knowledge-knowledge-base-details");

            migrationBuilder.DropTable(
                name: "knowledge-users");

            migrationBuilder.DropTable(
                name: "knowledge-chat-dialog");

            migrationBuilder.DropTable(
                name: "knowledge-file-storages");

            migrationBuilder.DropTable(
                name: "knowledge-knowledge-base");

            migrationBuilder.DropTable(
                name: "knowledge-chat-application");
        }
    }
}
