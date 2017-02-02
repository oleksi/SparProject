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

	select sr.SparRequestId, sr.StatusId, sr.RequestorFighterId, sr.RequestDate, sr.OpponentFighterId, sr.LastNegotiatorFighterId, case when sr.StatusId = 1 then sr.RequestDate else srh.HistoryTimestamp end as LastUpdateDate
	from SparRequests sr
	left join SparRequests_History srh on srh.SparRequestId = sr.SparRequestId and srh.HistoryTimestamp = (
		select max(srh1.HistoryTimestamp)
		from SparRequests_History srh1
		where srh1.SparRequestId = sr.SparRequestId
	)
	where (sr.StatusId = 1 or sr.StatusId = 2)
	and (
			(srh.HistoryTimestamp is null and sr.RequestDate > @SinceDate) or
			(srh.HistoryTimestamp is not null and srh.HistoryTimestamp > @SinceDate)
	)
GO
