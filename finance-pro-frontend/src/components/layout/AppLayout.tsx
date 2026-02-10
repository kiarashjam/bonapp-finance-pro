import React, { useState } from 'react';
import { Layout, Menu, Avatar, Dropdown, Typography, theme } from 'antd';
import {
  DashboardOutlined, BookOutlined, FileTextOutlined,
  DollarOutlined, ShopOutlined, SwapOutlined, BarChartOutlined,
  SettingOutlined, LogoutOutlined, UserOutlined, AuditOutlined,
} from '@ant-design/icons';
import { Outlet, useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';
import { logout } from '../../store/slices/authSlice';

const { Sider, Header, Content } = Layout;
const { Text } = Typography;

const menuItems = [
  { key: '/dashboard', icon: <DashboardOutlined />, label: 'Dashboard' },
  { key: '/sales', icon: <DollarOutlined />, label: 'Sales & Payments' },
  { key: '/accounts', icon: <BookOutlined />, label: 'Chart of Accounts' },
  { key: '/journals', icon: <AuditOutlined />, label: 'Journal Entries' },
  { key: '/expenses', icon: <ShopOutlined />, label: 'Expenses & Bills' },
  { key: '/invoices', icon: <FileTextOutlined />, label: 'Invoicing' },
  { key: '/reconciliation', icon: <SwapOutlined />, label: 'Reconciliation' },
  { key: '/reports', icon: <BarChartOutlined />, label: 'Reports' },
  { key: '/settings', icon: <SettingOutlined />, label: 'Settings' },
];

const AppLayout: React.FC = () => {
  const [collapsed, setCollapsed] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();
  const { user, dispatch } = useAuth();
  const { token: { colorBgContainer } } = theme.useToken();

  const handleLogout = () => { dispatch(logout()); navigate('/login'); };

  const userMenu = {
    items: [
      { key: 'profile', label: `${user?.firstName} ${user?.lastName}`, disabled: true },
      { key: 'org', label: user?.organizationName, disabled: true },
      { type: 'divider' as const },
      { key: 'logout', icon: <LogoutOutlined />, label: 'Logout', onClick: handleLogout },
    ],
  };

  const selectedKey = '/' + location.pathname.split('/')[1];

  return (
    <Layout style={{ minHeight: '100vh' }}>
      <Sider collapsible collapsed={collapsed} onCollapse={setCollapsed} theme="dark" width={240}>
        <div style={{ height: 64, display: 'flex', alignItems: 'center', justifyContent: 'center', borderBottom: '1px solid rgba(255,255,255,0.1)' }}>
          <Text strong style={{ color: '#fff', fontSize: collapsed ? 14 : 18 }}>
            {collapsed ? 'FP' : 'Finance Pro'}
          </Text>
        </div>
        <Menu
          theme="dark"
          mode="inline"
          selectedKeys={[selectedKey]}
          items={menuItems}
          onClick={({ key }) => navigate(key)}
        />
      </Sider>
      <Layout>
        <Header style={{ background: colorBgContainer, padding: '0 24px', display: 'flex', alignItems: 'center', justifyContent: 'space-between', borderBottom: '1px solid #f0f0f0' }}>
          <Text strong style={{ fontSize: 16 }}>{user?.organizationName || 'Finance Pro'}</Text>
          <Dropdown menu={userMenu} placement="bottomRight">
            <div style={{ cursor: 'pointer', display: 'flex', alignItems: 'center', gap: 8 }}>
              <Avatar icon={<UserOutlined />} />
              <Text>{user?.firstName}</Text>
            </div>
          </Dropdown>
        </Header>
        <Content style={{ margin: 24, padding: 24, background: colorBgContainer, borderRadius: 8, minHeight: 280 }}>
          <Outlet />
        </Content>
      </Layout>
    </Layout>
  );
};

export default AppLayout;
