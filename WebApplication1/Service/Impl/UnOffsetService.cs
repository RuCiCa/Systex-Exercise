using System.Collections.Generic;
using System.ComponentModel;
using WebApplication1.Common;
using WebApplication1.Repositories.Api;
using WebApplication1.Service.Api;
using WebApplication1.Service.Dtos;

namespace WebApplication1.Service.Impl
{
    public class UnOffsetService : IUnOffsetService
    {
        private readonly UnOffsetAccsum _unOffsetAccsum;
        private readonly IRepository _repository;


        public UnOffsetService(UnOffsetAccsum unOffsetAccsum, IRepository repository)
        {
            _unOffsetAccsum = unOffsetAccsum;
            _repository = repository;
        }

        /// <summary>
        /// �������{�l�q���Ӫѩ���
        /// </summary>
        /// <param name="unOffset">�s��TCNUD�H��CNAME��CPRICE</param>
        /// <returns>���\�|�^��unOffsetDetail�A�ΨӫO�s</returns>
        public UnOffsetDetail GetUnOffsetDetail(UnOffset unOffset)
        {
            TCNUD tcnud = unOffset.TCNUD;
            Logger.Log(0, "�}�l", $"�}�l�p��{unOffset.CNAME}������{�l�q �V �Ӫѩ��ӡA�e�U�Ѹ�{tcnud.DSEQ}-���渹�X{tcnud.DNO}");
            try
            {
                string stock = tcnud.STOCK;
                string stocknm = unOffset.CNAME;
                decimal? bqty = tcnud.BQTY;
                decimal? mprice = tcnud.PRICE;
                decimal? mamt = bqty * mprice;
                decimal? lastprice = unOffset.CPRICE;
                decimal? cost = tcnud.COST;
                decimal? estimateAmt = lastprice * tcnud.BQTY;
                decimal estFee = 0.001425m;
                decimal estTax = 0.003m;
                Logger.Log(2, "�ܼ�", $"����O���G{estFee}, �|�v���G{estTax}");
                decimal? estimateFee = estimateAmt * estFee;
                decimal? estimateTax = estimateAmt * estTax;
                decimal? marketvalue = estimateAmt - estimateFee - estimateTax;
                decimal? profit = marketvalue - cost;
                string pl_ratio;

                if (cost != 0m)
                {
                    pl_ratio = ((profit / cost) * 100).ToString() + "%";
                }
                else
                {
                    Logger.Log(3, "ĵ�i", "cost��0�A�L�k�p��pl_ratio");
                    pl_ratio = "NAN%";
                }

                UnOffsetDetail unOffsetDetail = new UnOffsetDetail()
                {
                    stock = stock,
                    stocknm = stocknm,
                    tdate = tcnud.TDATE,
                    ttype = "0",
                    ttypename = "�{�R",
                    bstype = "B",
                    dseq = tcnud.DSEQ,
                    dno = tcnud.DNO,
                    bqty = bqty,
                    mprice = mprice,
                    mamt = mamt,
                    lastprice = lastprice,
                    marketvalue = marketvalue,
                    fee = tcnud.FEE,
                    tax = 0,
                    cost = cost,
                    estimateAmt = estimateAmt,
                    estimateFee = estimateFee,
                    estimateTax = estimateTax,
                    profit = profit,
                    pl_ratio = pl_ratio
                };
                Logger.Log(0, "���\", $"{unOffset.CNAME}������{�l�q �V �Ӫѩ��ӡA�e�U�Ѹ�{tcnud.DSEQ}-���渹�X{tcnud.DNO}�p�⦨�\");
                return unOffsetDetail;
            }
            //�z�LerrorHandler�ӳB�zerrcode��errmsg
            catch (Exception ex)
            {
                //errorHandler("500", $"�p��{unOffset.CNAME}������{�l�q �V �Ӫѩ��ӡA�e�U�Ѹ�{tcnud.DSEQ}-���渹�X{tcnud.DNO}�ɥX�{���~");
                Logger.Log(4, "����", $"���~�T��: {ex}");
                return null;
            }
        }

        /// <summary>
        /// ����Ӫѥ���{�l�q
        /// </summary>
        /// <param name="bhno">�����q</param>
        /// <param name="cseq">�b��</param>
        /// <returns>���\�|�^��unOffsetDetailList�A�ΨӫO�sunOffsetDetail</returns>
        public async Task<List<UnOffsetDetail>> GetUnOffsetDetailList(string bhno, string cseq)
        {
            //�إߤ@��dict�A�M��ϥ�StringArrayComparer�ӹ�@��key��string[]�ˬd
            var unOffsetDetailList = new List<UnOffsetDetail>();
            var list = (await _repository.GetByTwoKey(bhno, cseq)).ToList();
            if (list.Count == 0)
            {
                Logger.Log(3, "ĵ�i", $"��Ʈw���䤣������q{bhno}�b��{cseq}���������");
                return [] ;
            }
            Logger.Log(0, "���\", $"�j�M��{bhno}�b��{cseq}��{list.Count}���������");

            //List LinQ
            foreach (var unOffset in list)
            {
                var unOffsetDetail = GetUnOffsetDetail(unOffset);

                if (unOffsetDetail == null)
                {
                    return null;
                }
                try
                {
                    unOffsetDetailList.Add(unOffsetDetail);
                }
                catch (Exception ex)
                {
                    Logger.Log(4, "����", $"���~�T��: {ex}");
                    //errorHandler("500", $"�p��{cName}���Ӫѥ���{�l�q�ɥX�{���~");
                    return null;
                }
            }

            //�^��list�A���W���Ӫѩ��Ӫ�list
            return unOffsetDetailList;
        }

        /// <summary>
        /// �Nlist�̭����Ҧ�UnOffsetDetail���ȥ[�`�A�����@�Ѳ���unOffsetSum(�Ӫѥ���{�l�q)
        /// </summary>
        /// <param name="list">�s���@�Ѳ����Ҧ�����{�l�q����</param>
        /// <returns>���\�|�^�ǳ�@�Ѳ���unOffsetSum�A�ΨӫO�s�Ӫѥ���{�l�q</returns>
        public UnOffsetSum GetUnOffsetSum(List<UnOffsetDetail> list)
        {
            try
            {
                string stock = list.FirstOrDefault()?.stock;
                string stocknm = list.FirstOrDefault()?.stocknm;
                decimal? bqty = list.Sum(t => t.bqty);
                decimal? cost = list.Sum(t => t.cost);
                decimal? avgprice = cost / bqty;
                decimal? marketvalue = list.Sum(t => t.marketvalue);
                decimal? estimateAmt = list.Sum(t => t.estimateAmt);
                decimal? estimateFee = list.Sum(t => t.estimateFee);
                decimal? estimateTax = list.Sum(t => t.estimateTax);
                decimal? profit = list.Sum(t => t.profit);
                decimal? fee = list.Sum(t => t.fee);
                decimal? tax = list.Sum(t => t.tax);
                decimal? amt = list.Sum(t => t.mamt);
                string pl_ratio;

                //UnOffsetDetail�w�g�����L���~log����
                if (cost != 0m)
                {
                    pl_ratio = ((profit / cost) * 100).ToString() + "%";
                }
                else
                {
                    pl_ratio = "NAN%";
                }

                UnOffsetSum unOffsetSum = new UnOffsetSum()
                {
                    stock = stock,
                    stocknm = stocknm,
                    ttype = "0",
                    ttypename = "�{�R",
                    bstype = "B",
                    bqty = bqty,
                    cost = cost,
                    avgprice = avgprice,
                    lastprice = list.FirstOrDefault()?.lastprice,
                    marketvalue = marketvalue,
                    estimateAmt = estimateAmt,
                    estimateFee = estimateFee,
                    estimateTax = estimateTax,
                    profit = profit,
                    pl_ratio = pl_ratio,
                    fee = fee,
                    tax = tax,
                    amt = amt,
                    unoffset_qtype_detail = list
                };

                return unOffsetSum;
            }
            catch (Exception ex)
            {
                Logger.Log(4, "����", $"���~�T��: {ex}");
                //errorHandler("500", $"�p��{stocknm}�Ӫѥ���{�l�q�ɥX�{���~");
                return null;
            }
        }

        /// <summary>
        /// �Nlist�̷ӪѲ���C��list�i�������@�@���UnOffsetSum�æs�Jlist
        /// </summary>
        /// <param name="list">�Ҧ����Ӫѩ���</param>
        /// <returns>���\�|�^��unOffsetSumList�A�ΨӫO�s�Ҧ����Ӫѥ���{�l�q</returns>
        public async Task<List<UnOffsetSum>> GetUnOffsetSumList(List<UnOffsetDetail> list)
        {
            List<UnOffsetSum> unOffsetSums = new List<UnOffsetSum>();
            var groupedUnOffsets = list.GroupBy(u => new { u.stock, u.stocknm });
            //�ھڤ@�تѲ��@�ӭӪѥ���{�v�q
            foreach (var group in groupedUnOffsets)
            {
                Logger.Log(0, "�}�l", $"�}�l�p��{group.Key.stocknm}���Ӫѥ���{�l�q");
                try
                {
                    UnOffsetSum unOffsetSum = GetUnOffsetSum(group.ToList());
                    if (unOffsetSum == null)
                    {
                        return null;
                    }
                    unOffsetSums.Add(unOffsetSum);
                }
                catch (Exception ex)
                {
                    Logger.Log(4, "����", $"���~�T��: {ex}");
                    //errorHandler("500", $"�p��{group.Key.stocknm}���Ӫѥ���{�l�q�ɥX�{���~");
                    return null;
                }
                Logger.Log(0, "���\", $"{group.Key.stocknm}���Ӫѥ���{�l�q�p�⦨�\");
            }
            return unOffsetSums;
        }

        /// <summary>
        /// �N�Ҧ����Ӫѥ���{�l�q�[�`�öǥX
        /// </summary>
        /// <param name="list">�Ҧ����Ӫѥ���{�l�q</param>
        /// <returns>���\�|�^��unoffset_qtype_accsum�A�ΨӫO�s�b�����`</returns>
        public async Task<UnOffsetAccsum> GetUnOffsetAccsum(List<UnOffsetSum> list)
        {
            Logger.Log(0, "�}�l", $"�}�l����ꤺ�Ҩ�-����{�l�q �b���J�`");
            UnOffsetAccsum unoffset_qtype_accsum = new UnOffsetAccsum();
            try
            {

                //��GetUnOffsetSum�άOGetUnOffsetDetail�X�{���D�ɷ|�^��null�A�]���T���w�g�g�n�A�ҥH�����^�ǥu�s�񦳿��~�T����
                if (list == null)
                {
                    return unoffset_qtype_accsum;
                }

                //�z�Llist���[�`�\��[�`�C�@��
                var bqty = list.Sum(t => t.bqty);
                var cost = list.Sum(t => t.cost);
                var marketvalue = list.Sum(t => t.marketvalue);
                var profit = list.Sum(t => t.profit);

                //�bUnOffsetDetail�w�g����pl_ratio�����~log�������F�A�ҥH�u�b�̭��O���@��
                string pl_ratio;
                if (cost != 0m)
                {
                    pl_ratio = ((profit / cost) * 100).ToString() + "%";
                }
                else
                {
                    pl_ratio = "NAN%";
                }

                var fee = list.Sum(t => t.fee);
                var tax = list.Sum(t => t.tax);
                var estimateAmt = list.Sum(t => t.estimateAmt);
                var estimateFee = list.Sum(t => t.estimateFee);
                var estimateTax = list.Sum(t => t.estimateTax);

                //�p�G���S���D�N�Nerrcode�]��0000�Aerrmsg�]�����\
                var errcode = "0000";
                var errmsg = "���\";

                unoffset_qtype_accsum = new UnOffsetAccsum
                {
                    errcode = errcode,
                    errmsg = errmsg,
                    bqty = bqty,
                    cost = cost,
                    marketvalue = marketvalue,
                    profit = profit,
                    pl_ratio = pl_ratio,
                    fee = fee,
                    tax = tax,
                    estimateAmt = estimateAmt,
                    estimateFee = estimateFee,
                    estimateTax = estimateTax,
                    unoffset_qtype_sum = list
                };
                Logger.Log(0, "���\", "�ꤺ�Ҩ�-����{�l�q �b���J�`������\");
                return unoffset_qtype_accsum;
            }
            catch (Exception ex)
            {
                unoffset_qtype_accsum.errcode = "500";
                unoffset_qtype_accsum.errmsg = "�ꤺ�Ҩ�-����{�l�q �b���J�`�������";
                unoffset_qtype_accsum.unoffset_qtype_sum = [];

                Logger.Log(4, "����", $"�ꤺ�Ҩ�-����{�l�q �b���J�`������ѡA���~�T���G{ex.Message}");

                return unoffset_qtype_accsum;
            }
        }

        /// <summary>
        /// �ͦ�����{�l�q�����~�T��
        /// </summary>
        /// <param name="errcode">���~�X</param>
        /// <param name="errmsg">���~�T��</param>
        /// <returns>���\�|�^��unoffset_qtype_accsum�A�ΨӫO�s�b�����`</returns>
        public async Task<UnOffsetAccsum> GetFailedUnOffsetAccsum(string errcode, string errmsg)
        {
            UnOffsetAccsum unoffset_qtype_accsum = new UnOffsetAccsum();

            unoffset_qtype_accsum.errcode = errcode;
            unoffset_qtype_accsum.errmsg = errmsg;
            unoffset_qtype_accsum.unoffset_qtype_sum = [];

            return unoffset_qtype_accsum;
        }
    }
}
