using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIMS_BD_IATI.Library.Parser.ParserIATIv2
{
    public interface ITransaction
    {
        transactionTransactiontype transactiontype { get; set; }

        transactionTransactiondate transactiondate { get; set; }

        currencyType value { get; set; }

        textRequiredType description { get; set; }

        transactionProviderorg providerorg { get; set; }

        transactionReceiverorg receiverorg { get; set; }

        transactionDisbursementchannel disbursementchannel { get; set; }

        transactionSector[] sector { get; set; }

        transactionRecipientcountry recipientcountry { get; set; }

        transactionRecipientregion recipientregion { get; set; }

        transactionFlowtype flowtype { get; set; }

        transactionFinancetype financetype { get; set; }

        transactionAidtype aidtype { get; set; }

        transactionTiedstatus tiedstatus { get; set; }

        bool humanitarian { get; set; }

        bool humanitarianSpecified { get; set; }

    }
}
