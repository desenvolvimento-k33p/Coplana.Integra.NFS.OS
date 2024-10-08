SELECT
OPCH."DocEntry" AS "DocEntryNFE",
OPCH."DocNum" AS "DocNumNFE",
OINV."Serial" AS "SerialSaída",
OPCH."Serial" AS "SerialEntrada",
OINV."DocDate" AS "DataSaida",
OBPL."BPLName" AS "FilialOrigem",
OBPL_FILD."BPLName" AS "FilialDestino"
FROM OINV
INNER JOIN OBPL
ON OINV."BPLId" = OBPL."BPLId"
INNER JOIN OBPL OBPL_FILD
ON OINV."CardCode" = OBPL_FILD."DflCust"
AND OBPL_FILD."BPLId" NOT IN (2)
LEFT JOIN OPCH
ON OPCH."CardCode" = OBPL."DflVendor"
AND OINV."Serial" = OPCH."Serial"
WHERE EXISTS (SELECT OBPL."DflCust" FROM OBPL WHERE OBPL."DflCust" = OINV."CardCode")
AND OINV.CANCELED = 'Y'
AND OPCH.CANCELED = 'N'
ORDER BY 1