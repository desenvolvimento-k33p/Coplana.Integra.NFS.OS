SELECT
TOP 1 "P"."DocType",
"P"."DocEntry",
"P"."KeyNfe" AS "KEYNFE",
"H"."Hist" AS "MENSAGEM_SEFAZ",
"P"."StatusId" AS "STATUS",
"S"."Description" AS "STATUS_NFE"
FROM
"DBInvOne"."Process" AS "P"
INNER JOIN "DBInvOne"."ProcessHist" AS "H" ON
"P"."BatchId" = "H"."BatchId"
LEFT JOIN "DBInvOne"."ProcessStatus" AS "S" ON
"S"."ID" = "P"."StatusId"
WHERE "P"."DocEntry" = '109'
AND "P"."DocType" = 19
AND "P"."CompanyId" = 24





SELECT
TOP 1 "P"."DocType",
"P"."DocEntry",
"P"."KeyNfe" AS "KEYNFE",
"H"."Hist" AS "MENSAGEM_SEFAZ",
"P"."StatusId" AS "STATUS",
"S"."Description" AS "STATUS_NFE"
FROM
"DBInvOne"."Process" AS "P"
INNER JOIN "DBInvOne"."ProcessHist" AS "H" ON
"P"."BatchId" = "H"."BatchId"
LEFT JOIN "DBInvOne"."ProcessStatus" AS "S" ON
"S"."ID" = "P"."StatusId"
WHERE "P"."DocEntry" = '109'
AND "P"."DocType" = 19
AND "P"."CompanyId" = 24
------------------------------------------------------------------------------------

UPDATE OPCH T 
SET T."U_NumPedTr" = null
FROM OWTR T
INNER JOIN WTR1 T1 ON T."DocEntry" = T1."DocEntry" 
WHERE T1."BaseRef" = '152'

---//////////UPDATE TESTES///////////////////////////////////////////////////////
select 
P."DocEntry",P."CompanyId",P."StatusId",P."DocType",P."BatchId" 
from "DBInvOne"."Process" P 
--INNER JOIN OINV V On V."DocEntry" = P."DocEntry"
where  
P."DocType" = 13 
and P."StatusId" = 4 
order by "BatchId";

update  "DBInvOne"."Process" 
set 
"DocEntry" = 33168,
"CompanyId" = 56 
WHERE "BatchId" = 1138;

update  "DBInvOne"."Process" 
set 
"DocEntry" = 33165,
"CompanyId" = 56 
WHERE "BatchId" = 21375;

update  "DBInvOne"."Process" 
set 
"DocEntry" = 33162,
"CompanyId" = 56 
WHERE "BatchId" = 21385;


update OINV SET "U_NumTransf" = NULL

--////////////////////////////////////////////////////////////////////////////

select 
V."DocNum",V."U_NumTransf",V."U_NumPedTr",P."DocEntry",P."CompanyId",P."StatusId",P."DocType",P."BatchId" 
from "DBInvOne"."Process" P 
INNER JOIN OINV V On V."DocEntry" = P."DocEntry"
where  
P."DocType" = 13 
and P."StatusId" = 4 
AND P."CompanyId" =56
order by "BatchId";





SELECT "Hist",* FROM "DBInvOne"."ProcessHist" WHERE "BatchId" =1138




select * from "DBInvOne"."ProcessStatus" ;
order by "BatchId";






















select * from OBPL


SELECT 
ENT.*
FROM "SBO_TaxOne"."Entidade" ENT 
INNER JOIN "DBInvOne"."Process" PRO 
ON ENT.ID = PRO."CompanyId" 
INNER JOIN OBPL 
ON ENT."BusinessPlaceId" = OBPL."BPLId" 
WHERE PRO."DocEntry" = 916


select ENT.*
FROM "SBO_TaxOne"."Entidade" ENT 
where ENT.ID in (51,56)