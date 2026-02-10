import React, { useEffect, useState } from 'react';
import { Table, Button, Typography, Tag, Row, Col, Statistic, Modal, Form, InputNumber, DatePicker, message } from 'antd';
import { PlusOutlined, CheckCircleOutlined } from '@ant-design/icons';
import { salesApi } from '../../api';
import { formatCurrency, formatDate, getStatusColor } from '../../utils/format';
import type { DailySalesSummaryDto, CloseDayPreviewDto } from '../../types';
import dayjs from 'dayjs';

const { Title } = Typography;

const SalesPage: React.FC = () => {
  const [summaries, setSummaries] = useState<DailySalesSummaryDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [manualModal, setManualModal] = useState(false);
  const [closeDayModal, setCloseDayModal] = useState(false);
  const [preview, setPreview] = useState<CloseDayPreviewDto | null>(null);
  const [selectedDate, setSelectedDate] = useState<string>('');
  const [form] = Form.useForm();

  const load = () => {
    setLoading(true);
    const end = dayjs().format('YYYY-MM-DD');
    const start = dayjs().subtract(30, 'day').format('YYYY-MM-DD');
    salesApi.getDailySummaries(start, end).then(res => setSummaries(res.data)).finally(() => setLoading(false));
  };

  useEffect(() => { load(); }, []);

  const handleManualEntry = async (values: any) => {
    try {
      await salesApi.createManualEntry({ ...values, date: values.date.format('YYYY-MM-DD') });
      message.success('Sales entry created'); setManualModal(false); form.resetFields(); load();
    } catch (err: any) { message.error(err.response?.data?.error || 'Failed'); }
  };

  const handleCloseDay = async (date: string) => {
    try {
      const res = await salesApi.getCloseDayPreview(date);
      setPreview(res.data); setSelectedDate(date); setCloseDayModal(true);
    } catch (err: any) { message.error(err.response?.data?.error || 'Failed to get preview'); }
  };

  const confirmCloseDay = async () => {
    try {
      await salesApi.closeDay(selectedDate);
      message.success('Day closed successfully'); setCloseDayModal(false); load();
    } catch (err: any) { message.error(err.response?.data?.error || 'Failed'); }
  };

  const columns = [
    { title: 'Date', dataIndex: 'date', key: 'date', width: 110, render: (d: string) => formatDate(d) },
    { title: 'Gross Sales', dataIndex: 'grossSales', key: 'gross', align: 'right' as const, render: (v: number) => formatCurrency(v) },
    { title: 'Net Sales', dataIndex: 'netSales', key: 'net', align: 'right' as const, render: (v: number) => <strong>{formatCurrency(v)}</strong> },
    { title: 'Cash', dataIndex: 'cashSales', key: 'cash', align: 'right' as const, render: (v: number) => formatCurrency(v) },
    { title: 'Card', dataIndex: 'cardSales', key: 'card', align: 'right' as const, render: (v: number) => formatCurrency(v) },
    { title: 'Txns', dataIndex: 'transactionCount', key: 'txns', align: 'right' as const },
    { title: 'Avg Check', dataIndex: 'averageCheck', key: 'avg', align: 'right' as const, render: (v: number) => formatCurrency(v) },
    { title: 'Status', dataIndex: 'dayStatus', key: 'status', render: (s: string) => <Tag color={getStatusColor(s)}>{s}</Tag> },
    { title: '', key: 'actions', width: 120, render: (_: any, r: DailySalesSummaryDto) => r.dayStatus === 'Open' ? <Button size="small" icon={<CheckCircleOutlined />} onClick={() => handleCloseDay(r.date)}>Close Day</Button> : null },
  ];

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 16 }}>
        <Title level={3} style={{ margin: 0 }}>Sales & Payments</Title>
        <Button type="primary" icon={<PlusOutlined />} onClick={() => setManualModal(true)}>Manual Entry</Button>
      </div>
      <Table dataSource={summaries} columns={columns} rowKey="id" loading={loading} size="small" pagination={false} />

      {/* Manual Entry Modal */}
      <Modal title="Manual Sales Entry" open={manualModal} onCancel={() => setManualModal(false)} onOk={() => form.submit()} width={600} okText="Save">
        <Form form={form} layout="vertical" onFinish={handleManualEntry}>
          <Form.Item name="date" label="Date" rules={[{ required: true }]}><DatePicker style={{ width: '100%' }} /></Form.Item>
          <Row gutter={12}>
            <Col span={12}><Form.Item name="grossSales" label="Gross Sales" rules={[{ required: true }]}><InputNumber style={{ width: '100%' }} min={0} precision={2} /></Form.Item></Col>
            <Col span={12}><Form.Item name="totalTips" label="Tips" initialValue={0}><InputNumber style={{ width: '100%' }} min={0} precision={2} /></Form.Item></Col>
          </Row>
          <Row gutter={12}>
            <Col span={8}><Form.Item name="cashSales" label="Cash" initialValue={0}><InputNumber style={{ width: '100%' }} min={0} precision={2} /></Form.Item></Col>
            <Col span={8}><Form.Item name="cardSales" label="Card" initialValue={0}><InputNumber style={{ width: '100%' }} min={0} precision={2} /></Form.Item></Col>
            <Col span={8}><Form.Item name="onlineSales" label="Online" initialValue={0}><InputNumber style={{ width: '100%' }} min={0} precision={2} /></Form.Item></Col>
          </Row>
          <Row gutter={12}>
            <Col span={8}><Form.Item name="vatStandard" label="VAT 8.1%" initialValue={0}><InputNumber style={{ width: '100%' }} min={0} precision={2} /></Form.Item></Col>
            <Col span={8}><Form.Item name="vatReduced" label="VAT 2.6%" initialValue={0}><InputNumber style={{ width: '100%' }} min={0} precision={2} /></Form.Item></Col>
            <Col span={8}><Form.Item name="vatHospitality" label="VAT 3.8%" initialValue={0}><InputNumber style={{ width: '100%' }} min={0} precision={2} /></Form.Item></Col>
          </Row>
          <Row gutter={12}>
            <Col span={8}><Form.Item name="otherSales" label="Other" initialValue={0}><InputNumber style={{ width: '100%' }} min={0} precision={2} /></Form.Item></Col>
            <Col span={8}><Form.Item name="transactionCount" label="Transactions" initialValue={0}><InputNumber style={{ width: '100%' }} min={0} /></Form.Item></Col>
            <Col span={8}><Form.Item name="guestCount" label="Guests" initialValue={0}><InputNumber style={{ width: '100%' }} min={0} /></Form.Item></Col>
          </Row>
        </Form>
      </Modal>

      {/* Close Day Preview Modal */}
      <Modal title="Close Day Preview" open={closeDayModal} onCancel={() => setCloseDayModal(false)} onOk={confirmCloseDay} okText="Confirm & Close Day" okButtonProps={{ danger: true }}>
        {preview && (
          <div>
            <p>Are you sure you want to close <strong>{formatDate(preview.date)}</strong>? This will generate journal entries and lock the day.</p>
            <Row gutter={16}>
              <Col span={12}><Statistic title="Gross Sales" value={preview.grossSales} prefix="CHF" precision={2} /></Col>
              <Col span={12}><Statistic title="Net Sales" value={preview.netSales} prefix="CHF" precision={2} /></Col>
              <Col span={12}><Statistic title="Cash" value={preview.cashSales} prefix="CHF" precision={2} /></Col>
              <Col span={12}><Statistic title="Card" value={preview.cardSales} prefix="CHF" precision={2} /></Col>
            </Row>
          </div>
        )}
      </Modal>
    </div>
  );
};

export default SalesPage;
