USE [SparProject]
GO

/****** Object:  StoredProcedure [dbo].[SparRequest_GetNotesBySparRequestId]    Script Date: 2/1/2017 9:27:20 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[SparRequest_GetAllActiveSince]
	 @SinceDate datetime
as

	select sr.SparRequestId, sr.StatusId, sr.RequestorFighterId, sr.RequestDate, sr.OpponentFighterId, sr.LastNegotiatorFighterId, sr.LastUpdateDate
	from SparRequests sr
	where (sr.StatusId = 1 or sr.StatusId = 2)
	and sr.LastUpdateDate > @SinceDate

GO
