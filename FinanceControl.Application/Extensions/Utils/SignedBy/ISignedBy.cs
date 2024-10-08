using FinanceControl.Application.Extensions.Utils.Email;
using FinanceControl.Application.Services.Transactions.Model;
using FinanceControl.Application.Services.User.Model;

namespace FinanceControl.Application.Extensions.Utils.SignedBy
{
    public interface ISignedBy
    {
        void SignedByCard(string userName, FamilyMemberModel familyMember, TransactionsModel model, string nameCard, IEmail email, string subjectEmail);
        void SignedByCardDebit(string userName, FamilyMemberModel familyMember, TransactionsModel model, string nameCard, IEmail email, string subjectEmail);
        void SignedByBankSlip(string userName, FamilyMemberModel familyMember, TransactionsModel model, string name, IEmail email, string subjectEmail);
        void SignedByPix(string userName, FamilyMemberModel familyMember, TransactionsModel model, string name, IEmail email, string subjectEmail);
    }
}
