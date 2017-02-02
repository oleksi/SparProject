create procedure SparRequest_GetNotesBySparRequestId
	 @SparRequestId nvarchar(128)
as

	select sr.MemberId, sr.NoteDate, sr.SparNotes
	from (
		select sr.SparRequestId, srh.LastNegotiatorFighterId as MemberId, case when srh.StatusId = 1 then srh.RequestDate else srhd.HistoryTimestamp end as NoteDate, srh.SparNotes
		from SparRequests sr
		join SparRequests_History srh on srh.SparRequestId = sr.SparRequestId
		left join SparRequests_History srhd on srhd.SparRequestId = sr.SparRequestId and srhd.HistoryTimestamp = (
			select max(srh1.HistoryTimestamp)
			from SparRequests_History srh1
			where srh1.SparRequestId = sr.SparRequestId
			and srh1.HistoryTimestamp < srh.HistoryTimestamp
		)
		where sr.SparRequestId = @SparRequestId

		union

		select sr.SparRequestId, sr.LastNegotiatorFighterId as MemberId, case when srh.HistoryTimestamp is not null then srh.HistoryTimestamp else sr.RequestDate end as NoteDate, sr.SparNotes
		from SparRequests sr
		left join SparRequests_History srh on srh.SparRequestId = sr.SparRequestId and srh.HistoryTimestamp = (
			select max(srh1.HistoryTimestamp)
			from SparRequests_History srh1
			where srh1.SparRequestId = sr.SparRequestId
		)
		where sr.SparRequestId = @SparRequestId and sr.StatusId = 2
	) sr
	where sr.SparNotes is not null and sr.SparNotes <> ''
	order by NoteDate