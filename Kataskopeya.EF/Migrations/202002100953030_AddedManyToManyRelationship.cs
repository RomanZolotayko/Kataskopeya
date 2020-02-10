namespace Kataskopeya.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedManyToManyRelationship : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FaceImages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Face = c.Binary(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserFaceImages",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        FaceImageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.FaceImageId })
                .ForeignKey("dbo.FaceImages", t => t.FaceImageId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.FaceImageId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserFaceImages", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserFaceImages", "FaceImageId", "dbo.FaceImages");
            DropIndex("dbo.UserFaceImages", new[] { "FaceImageId" });
            DropIndex("dbo.UserFaceImages", new[] { "UserId" });
            DropTable("dbo.UserFaceImages");
            DropTable("dbo.FaceImages");
        }
    }
}
