USE [master]
GO
/****** Object:  Database [coursework]    Script Date: 20.11.2023 11:44:21 ******/
CREATE DATABASE [coursework]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'coursework', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\coursework.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'coursework_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\coursework_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [coursework] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [coursework].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [coursework] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [coursework] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [coursework] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [coursework] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [coursework] SET ARITHABORT OFF 
GO
ALTER DATABASE [coursework] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [coursework] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [coursework] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [coursework] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [coursework] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [coursework] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [coursework] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [coursework] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [coursework] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [coursework] SET  DISABLE_BROKER 
GO
ALTER DATABASE [coursework] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [coursework] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [coursework] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [coursework] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [coursework] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [coursework] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [coursework] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [coursework] SET RECOVERY FULL 
GO
ALTER DATABASE [coursework] SET  MULTI_USER 
GO
ALTER DATABASE [coursework] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [coursework] SET DB_CHAINING OFF 
GO
ALTER DATABASE [coursework] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [coursework] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [coursework] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [coursework] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'coursework', N'ON'
GO
ALTER DATABASE [coursework] SET QUERY_STORE = ON
GO
ALTER DATABASE [coursework] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [coursework]
GO
/****** Object:  User [korhovvv]    Script Date: 20.11.2023 11:44:21 ******/
CREATE USER [korhovvv] FOR LOGIN [korhovvv] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  Table [dbo].[Calculation]    Script Date: 20.11.2023 11:44:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Calculation](
	[calculationId] [int] IDENTITY(1,1) NOT NULL,
	[workerId] [int] NOT NULL,
	[title] [varchar](50) NOT NULL,
	[dateOrder] [date] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[calculationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Features]    Script Date: 20.11.2023 11:44:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Features](
	[featuresId] [int] IDENTITY(1,1) NOT NULL,
	[typeParsing] [varchar](50) NOT NULL,
	[title] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[featuresId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Features_Materials]    Script Date: 20.11.2023 11:44:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Features_Materials](
	[featuresId] [int] NOT NULL,
	[sidingId] [int] NOT NULL,
	[value] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[featuresId] ASC,
	[sidingId] ASC,
	[value] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Materials_Calculation]    Script Date: 20.11.2023 11:44:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Materials_Calculation](
	[sidingId] [int] NOT NULL,
	[calculationId] [int] NOT NULL,
	[count] [decimal](10, 2) NOT NULL,
	[currentPrice] [decimal](10, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[sidingId] ASC,
	[calculationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 20.11.2023 11:44:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[roleId] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[roleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Siding]    Script Date: 20.11.2023 11:44:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Siding](
	[sidingId] [int] IDENTITY(1,1) NOT NULL,
	[title] [varchar](50) NOT NULL,
	[description] [varchar](255) NOT NULL,
	[price] [decimal](10, 2) NOT NULL,
	[image] [varbinary](max) NULL,
 CONSTRAINT [PK__Siding__D7C3C0FBFE3A2790] PRIMARY KEY CLUSTERED 
(
	[sidingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Walls]    Script Date: 20.11.2023 11:44:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Walls](
	[wallId] [int] IDENTITY(1,1) NOT NULL,
	[calculationId] [int] NOT NULL,
	[length] [decimal](4, 1) NOT NULL,
	[width] [decimal](4, 1) NOT NULL,
	[count] [tinyint] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[wallId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Windows]    Script Date: 20.11.2023 11:44:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Windows](
	[windowId] [int] IDENTITY(1,1) NOT NULL,
	[calculationId] [int] NOT NULL,
	[length] [decimal](4, 1) NOT NULL,
	[width] [decimal](4, 1) NOT NULL,
	[count] [tinyint] NOT NULL,
	[isDoor] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[windowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WorkerInformation]    Script Date: 20.11.2023 11:44:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WorkerInformation](
	[workerInformation] [int] IDENTITY(1,1) NOT NULL,
	[workerId] [int] NOT NULL,
	[firstName] [varchar](50) NOT NULL,
	[secondName] [varchar](50) NOT NULL,
	[lastName] [varchar](50) NOT NULL,
	[email] [varchar](50) NULL,
	[phone] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[workerInformation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Workers]    Script Date: 20.11.2023 11:44:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Workers](
	[workerId] [int] IDENTITY(1,1) NOT NULL,
	[roleId] [int] NOT NULL,
	[login] [varchar](50) NOT NULL,
	[password] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[workerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Calculation]  WITH CHECK ADD  CONSTRAINT [FKCalculation645376] FOREIGN KEY([workerId])
REFERENCES [dbo].[Workers] ([workerId])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[Calculation] CHECK CONSTRAINT [FKCalculation645376]
GO
ALTER TABLE [dbo].[Features_Materials]  WITH CHECK ADD  CONSTRAINT [FK_Features_Materials_Features] FOREIGN KEY([featuresId])
REFERENCES [dbo].[Features] ([featuresId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Features_Materials] CHECK CONSTRAINT [FK_Features_Materials_Features]
GO
ALTER TABLE [dbo].[Features_Materials]  WITH CHECK ADD  CONSTRAINT [FK_Features_Materials_Siding] FOREIGN KEY([sidingId])
REFERENCES [dbo].[Siding] ([sidingId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Features_Materials] CHECK CONSTRAINT [FK_Features_Materials_Siding]
GO
ALTER TABLE [dbo].[Materials_Calculation]  WITH CHECK ADD  CONSTRAINT [FKMaterials_109362] FOREIGN KEY([calculationId])
REFERENCES [dbo].[Calculation] ([calculationId])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[Materials_Calculation] CHECK CONSTRAINT [FKMaterials_109362]
GO
ALTER TABLE [dbo].[Materials_Calculation]  WITH CHECK ADD  CONSTRAINT [FKMaterials_148670] FOREIGN KEY([sidingId])
REFERENCES [dbo].[Siding] ([sidingId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Materials_Calculation] CHECK CONSTRAINT [FKMaterials_148670]
GO
ALTER TABLE [dbo].[Walls]  WITH CHECK ADD  CONSTRAINT [FKWalls123685] FOREIGN KEY([calculationId])
REFERENCES [dbo].[Calculation] ([calculationId])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[Walls] CHECK CONSTRAINT [FKWalls123685]
GO
ALTER TABLE [dbo].[Windows]  WITH CHECK ADD  CONSTRAINT [FKWindows682599] FOREIGN KEY([calculationId])
REFERENCES [dbo].[Calculation] ([calculationId])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[Windows] CHECK CONSTRAINT [FKWindows682599]
GO
ALTER TABLE [dbo].[WorkerInformation]  WITH CHECK ADD  CONSTRAINT [FKWorkerInfo191972] FOREIGN KEY([workerId])
REFERENCES [dbo].[Workers] ([workerId])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[WorkerInformation] CHECK CONSTRAINT [FKWorkerInfo191972]
GO
ALTER TABLE [dbo].[Workers]  WITH CHECK ADD  CONSTRAINT [FKWorkers125598] FOREIGN KEY([roleId])
REFERENCES [dbo].[Roles] ([roleId])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[Workers] CHECK CONSTRAINT [FKWorkers125598]
GO
USE [master]
GO
ALTER DATABASE [coursework] SET  READ_WRITE 
GO
