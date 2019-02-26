using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Nongzhsh.JobHub.Migrations
{
    public partial class Add_Bolgging_Module : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpEntityPropertyChanges_AbpEntityChanges_EntityChangeId1",
                table: "AbpEntityPropertyChanges");

            migrationBuilder.DropIndex(
                name: "IX_AbpEntityPropertyChanges_EntityChangeId1",
                table: "AbpEntityPropertyChanges");

            migrationBuilder.DropColumn(
                name: "EntityChangeId1",
                table: "AbpEntityPropertyChanges");

            migrationBuilder.CreateTable(
                name: "BlgBlogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ExtraProperties = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<Guid>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierId = table.Column<Guid>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    ShortName = table.Column<string>(maxLength: 32, nullable: false),
                    Description = table.Column<string>(maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlgBlogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlgTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ExtraProperties = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<Guid>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierId = table.Column<Guid>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    BlogId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    Description = table.Column<string>(maxLength: 512, nullable: true),
                    UsageCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlgTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlgUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    TenantId = table.Column<Guid>(nullable: true),
                    UserName = table.Column<string>(maxLength: 256, nullable: false),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    Name = table.Column<string>(maxLength: 64, nullable: true),
                    Surname = table.Column<string>(maxLength: 64, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false, defaultValue: false),
                    PhoneNumber = table.Column<string>(maxLength: 16, nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false, defaultValue: false),
                    ExtraProperties = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlgUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlgPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ExtraProperties = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<Guid>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierId = table.Column<Guid>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    BlogId = table.Column<Guid>(nullable: false),
                    Url = table.Column<string>(maxLength: 64, nullable: false),
                    CoverImage = table.Column<string>(nullable: false),
                    Title = table.Column<string>(maxLength: 512, nullable: false),
                    Content = table.Column<string>(maxLength: 1048576, nullable: true),
                    ReadCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlgPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlgPosts_BlgBlogs_BlogId",
                        column: x => x.BlogId,
                        principalTable: "BlgBlogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlgComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ExtraProperties = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<Guid>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierId = table.Column<Guid>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    PostId = table.Column<Guid>(nullable: false),
                    RepliedCommentId = table.Column<Guid>(nullable: true),
                    Text = table.Column<string>(maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlgComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlgComments_BlgPosts_PostId",
                        column: x => x.PostId,
                        principalTable: "BlgPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlgComments_BlgComments_RepliedCommentId",
                        column: x => x.RepliedCommentId,
                        principalTable: "BlgComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BlgPostTags",
                columns: table => new
                {
                    PostId = table.Column<Guid>(nullable: false),
                    TagId = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlgPostTags", x => new { x.PostId, x.TagId });
                    table.ForeignKey(
                        name: "FK_BlgPostTags_BlgPosts_PostId",
                        column: x => x.PostId,
                        principalTable: "BlgPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlgPostTags_BlgTags_TagId",
                        column: x => x.TagId,
                        principalTable: "BlgTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlgComments_PostId",
                table: "BlgComments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_BlgComments_RepliedCommentId",
                table: "BlgComments",
                column: "RepliedCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_BlgPosts_BlogId",
                table: "BlgPosts",
                column: "BlogId");

            migrationBuilder.CreateIndex(
                name: "IX_BlgPostTags_TagId",
                table: "BlgPostTags",
                column: "TagId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlgComments");

            migrationBuilder.DropTable(
                name: "BlgPostTags");

            migrationBuilder.DropTable(
                name: "BlgUsers");

            migrationBuilder.DropTable(
                name: "BlgPosts");

            migrationBuilder.DropTable(
                name: "BlgTags");

            migrationBuilder.DropTable(
                name: "BlgBlogs");

            migrationBuilder.AddColumn<Guid>(
                name: "EntityChangeId1",
                table: "AbpEntityPropertyChanges",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpEntityPropertyChanges_EntityChangeId1",
                table: "AbpEntityPropertyChanges",
                column: "EntityChangeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpEntityPropertyChanges_AbpEntityChanges_EntityChangeId1",
                table: "AbpEntityPropertyChanges",
                column: "EntityChangeId1",
                principalTable: "AbpEntityChanges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
