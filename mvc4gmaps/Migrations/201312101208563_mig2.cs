namespace MVC4GMAPS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Locations", "Lat", c => c.Double(nullable: true));
            AlterColumn("dbo.Locations", "Lon", c => c.Double(nullable: true));
        }
        
        public override void Down()
        {
        }
    }
}
