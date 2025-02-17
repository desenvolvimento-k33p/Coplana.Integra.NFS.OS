SELECT DISTINCT
ODRF."DocEntry" AS "DocEntry"
--ODRF."DocNum" AS "DocNumNFE",
--OINV."Serial" AS "SerialSaída",
--ODRF."Serial" AS "SerialEntrada",
--OINV."DocDate" AS "DataSaida",
--OBPL."BPLName" AS "FilialOrigem",
--OBPL_FILD."BPLName" AS "FilialDestino"
FROM OINV
INNER JOIN OBPL ON OINV."BPLId" = OBPL."BPLId"
INNER JOIN OBPL OBPL_FILD ON OINV."CardCode" = OBPL_FILD."DflCust" AND OBPL_FILD."BPLId" NOT IN (2)
INNER JOIN ODRF ON ODRF."CardCode" = OBPL."DflVendor" AND OINV."Serial" = ODRF."Serial" AND OINV."SeriesStr" = ODRF."SeriesStr" AND ODRF."ObjType" = 18
WHERE EXISTS (SELECT OBPL."DflCust" FROM OBPL WHERE OBPL."DflCust" = OINV."CardCode")
AND OINV.CANCELED = 'Y'
AND ODRF.CANCELED <> 'Y'
AND NOT EXISTS (SELECT * FROM OINV OINV_CAN WHERE OINV."CardCode" = OINV_CAN."CardCode" AND OINV."Serial" = OINV_CAN."Serial" AND OINV."SeriesStr" = OINV_CAN."SeriesStr" AND OINV_CAN."CANCELED" = 'N' )
ORDER BY 1