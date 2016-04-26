using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS_BD_IATI.DAL
{

    public partial class tblProjectFundingCommitment : ITransaction
    {
        public transactionTransactiontype transactiontype { get; set; }

        public transactionTransactiondate transactiondate { get; set; }

        public currencyType value { get; set; }

        public textRequiredType description { get; set; }

        public transactionProviderorg providerorg { get; set; }

        public transactionReceiverorg receiverorg { get; set; }

        public transactionDisbursementchannel disbursementchannel { get; set; }

        public transactionSector[] sector { get; set; }

        public transactionRecipientcountry recipientcountry { get; set; }

        public transactionRecipientregion recipientregion { get; set; }

        public transactionFlowtype flowtype { get; set; }

        public transactionFinancetype financetype { get; set; }

        public transactionAidtype aidtype { get; set; }

        public transactionTiedstatus tiedstatus { get; set; }

        public bool humanitarian { get; set; }

        public bool humanitarianSpecified { get; set; }

    }
}