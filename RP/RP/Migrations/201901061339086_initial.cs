namespace RP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CityName = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CompanyDetails",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CompanyName = c.String(),
                        CompanyType = c.String(),
                        OwnerName = c.String(),
                        Address = c.String(),
                        Contact = c.String(),
                        Email = c.String(),
                        FaxNo = c.String(),
                        IsoNo = c.String(),
                        BusinessTag = c.String(),
                        LandMark = c.String(),
                        LatLog = c.String(),
                        Name = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Contacts",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Designation = c.String(),
                        Mobile = c.Int(nullable: false),
                        Address = c.String(),
                        City = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.InwardOutwards",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        IsInward = c.Boolean(nullable: false),
                        City = c.Long(nullable: false),
                        InvoiceNo = c.String(),
                        ItemId = c.Long(nullable: false),
                        Unit = c.Long(nullable: false),
                        Quantity = c.Long(nullable: false),
                        InwardUserId = c.String(),
                        ReturnDate = c.DateTime(),
                        SiteName = c.String(),
                        ApiKey = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ItemMasters",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ItemName = c.String(),
                        Unit = c.String(),
                        Quantity = c.Int(nullable: false),
                        Manager_ID = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        StId = c.Int(nullable: false, identity: true),
                        StName = c.String(nullable: false),
                        StAddress = c.String(),
                        MobileNo = c.String(nullable: false),
                        test = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.StId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Guid(nullable: false),
                        UserName = c.String(),
                        Email = c.String(),
                        Password = c.String(),
                        Status = c.String(),
                        Apikey = c.String(),
                        Views = c.Boolean(nullable: false),
                        Insert = c.Boolean(nullable: false),
                        Update = c.Boolean(nullable: false),
                        Delete = c.Boolean(nullable: false),
                        ContactId = c.Long(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contacts", t => t.ContactId, cascadeDelete: true)
                .Index(t => t.ContactId);
            
            CreateTable(
                "dbo.Works",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CaseNo = c.String(),
                        UserId = c.String(),
                        City = c.Long(nullable: false),
                        CaseSite = c.String(),
                        CaseImage = c.String(),
                        Landmark = c.String(),
                        Length = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Width = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Ladditude = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Longitude = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BeforeDate = c.DateTime(),
                        AfterDate = c.DateTime(),
                        After = c.String(),
                        Before = c.String(),
                        ApiKey = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "ContactId", "dbo.Contacts");
            DropIndex("dbo.Users", new[] { "ContactId" });
            DropTable("dbo.Works");
            DropTable("dbo.Users");
            DropTable("dbo.Students");
            DropTable("dbo.ItemMasters");
            DropTable("dbo.InwardOutwards");
            DropTable("dbo.Contacts");
            DropTable("dbo.CompanyDetails");
            DropTable("dbo.Cities");
        }
    }
}
