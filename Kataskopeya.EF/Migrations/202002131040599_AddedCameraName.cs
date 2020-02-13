namespace Kataskopeya.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCameraName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cameras", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cameras", "Name");
        }
    }
}
