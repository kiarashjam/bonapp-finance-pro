import React from 'react';
import { Form, Input, Button, Card, Typography, Alert, Divider } from 'antd';
import { UserOutlined, LockOutlined, MailOutlined, BankOutlined } from '@ant-design/icons';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';
import { register, clearError } from '../../store/slices/authSlice';

const { Title, Text } = Typography;

const RegisterPage: React.FC = () => {
  const { loading, error, dispatch } = useAuth();
  const navigate = useNavigate();

  const onFinish = async (values: any) => {
    const result = await dispatch(register(values));
    if (register.fulfilled.match(result)) navigate('/dashboard');
  };

  return (
    <div style={{ minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center', background: '#f5f5f5' }}>
      <Card style={{ width: 480, boxShadow: '0 2px 8px rgba(0,0,0,0.09)' }}>
        <div style={{ textAlign: 'center', marginBottom: 24 }}>
          <Title level={2} style={{ marginBottom: 4 }}>Create Account</Title>
          <Text type="secondary">Start managing your restaurant finances</Text>
        </div>
        {error && <Alert message={error} type="error" showIcon closable onClose={() => dispatch(clearError())} style={{ marginBottom: 16 }} />}
        <Form layout="vertical" onFinish={onFinish} size="large">
          <Form.Item name="organizationName" rules={[{ required: true, message: 'Restaurant name is required' }]}>
            <Input prefix={<BankOutlined />} placeholder="Restaurant / Business Name" />
          </Form.Item>
          <div style={{ display: 'flex', gap: 12 }}>
            <Form.Item name="firstName" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Input prefix={<UserOutlined />} placeholder="First Name" />
            </Form.Item>
            <Form.Item name="lastName" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Input prefix={<UserOutlined />} placeholder="Last Name" />
            </Form.Item>
          </div>
          <Form.Item name="email" rules={[{ required: true, type: 'email' }]}>
            <Input prefix={<MailOutlined />} placeholder="Email" />
          </Form.Item>
          <Form.Item name="password" rules={[{ required: true, min: 8, message: 'At least 8 characters' }]}>
            <Input.Password prefix={<LockOutlined />} placeholder="Password" />
          </Form.Item>
          <Form.Item>
            <Button type="primary" htmlType="submit" loading={loading} block>Create Account</Button>
          </Form.Item>
        </Form>
        <Divider />
        <div style={{ textAlign: 'center' }}>
          <Text>Already have an account? <Link to="/login">Sign In</Link></Text>
        </div>
      </Card>
    </div>
  );
};

export default RegisterPage;
