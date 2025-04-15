
import React, { useState } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { 
  Rocket, 
  Home, 
  ClipboardList, 
  Settings, 
  HelpCircle, 
  LogOut,
  Package,
  PackageCheck,
  Clock,
  User,
  Plane
} from 'lucide-react';
import { Button } from '@/components/ui/button';
import { useAuth } from '@/contexts/AuthContext';

const navItems = [
  {
    roles: ['customer'],
    title: 'Kundmeny',
    items: [
      { icon: Home, label: 'Översikt', href: '/dashboard/overview' },
      { icon: Package, label: 'Boka Frakt', href: '/dashboard/book' },
      { icon: PackageCheck, label: 'Pågående Leveranser', href: '/dashboard/ongoing' },
      { icon: Clock, label: 'Tidigare Leveranser', href: '/dashboard/previous' },
    ]
  },
  {
    roles: ['pilot'],
    title: 'Pilotmeny',
    items: [
      { icon: Home, label: 'Översikt', href: '/dashboard/overview' },
      { icon: Rocket, label: 'Tilldelade Frakter', href: '/dashboard/assigned' },
    ]
  },
  {
    roles: ['admin'],
    title: 'Adminmeny',
    items: [
      { icon: Home, label: 'Översikt', href: '/dashboard/overview' },
      { icon: ClipboardList, label: 'Fraktöversikt', href: '/dashboard/shipments' },
      { icon: User, label: 'Hantera Piloter', href: '/dashboard/pilots' },
      // Removed "Tilldela frakter" item
    ]
  },
  {
    roles: ['customer', 'pilot', 'admin'],
    title: 'Övrigt',
    items: [
      { icon: Settings, label: 'Inställningar', href: '/dashboard/settings' },
      { icon: HelpCircle, label: 'Hjälp', href: '/faq' },
    ]
  }
];

interface DashboardSidebarProps {
  isMobile?: boolean;
  onClose?: () => void;
}

const DashboardSidebar: React.FC<DashboardSidebarProps> = ({ isMobile = false, onClose }) => {
  const location = useLocation();
  const navigate = useNavigate();
  const [isOpen, setIsOpen] = useState(true);
  const { user, logout } = useAuth();
  
  const userRole = user?.role || 'customer';

  const toggleSidebar = () => {
    setIsOpen(!isOpen);
  };

  const handleItemClick = () => {
    if (isMobile && onClose) {
      onClose();
    }
  };

  const handleLogout = () => {
    logout();
    navigate('/');
  };

  return (
    <aside className={`h-screen bg-space-primary border-r border-space-secondary/30 ${isOpen ? 'w-64' : 'w-20'} transition-all duration-300 flex flex-col`}>
      <div className="p-4 border-b border-space-secondary/30 flex items-center justify-between">
        <div className={`flex items-center ${!isOpen && 'justify-center w-full'}`}>
          <Rocket className={`h-6 w-6 text-space-accent-purple ${!isOpen && 'mx-auto'}`} />
          {isOpen && <span className="ml-2 text-xl font-bold space-gradient-text">CosmoCargo™</span>}
        </div>
        {!isMobile && (
          <Button 
            variant="ghost" 
            size="sm" 
            className="text-space-text-secondary" 
            onClick={toggleSidebar}
          >
            {isOpen ? (
              <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <path d="M15 18l-6-6 6-6" />
              </svg>
            ) : (
              <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <path d="M9 18l6-6-6-6" />
              </svg>
            )}
          </Button>
        )}
      </div>

      <div className="flex-1 overflow-y-auto py-4 px-3">
        {navItems
          .filter(section => section.roles.includes(userRole))
          .map((section, i) => (
            <div key={i} className="mb-6">
              {isOpen && (
                <h3 className="px-3 mb-2 text-xs font-semibold text-space-text-secondary uppercase tracking-wider">
                  {section.title}
                </h3>
              )}
              <ul className="space-y-1">
                {section.items.map((item, j) => (
                  <li key={j}>
                    <Link
                      to={item.href}
                      className={`flex items-center p-2 rounded-md transition-colors ${
                        location.pathname === item.href
                          ? 'bg-space-secondary text-space-accent-purple'
                          : 'text-space-text-primary hover:bg-space-secondary/50'
                      }`}
                      onClick={handleItemClick}
                    >
                      <item.icon className={`h-5 w-5 ${!isOpen && 'mx-auto'}`} />
                      {isOpen && <span className="ml-3">{item.label}</span>}
                    </Link>
                  </li>
                ))}
              </ul>
            </div>
          ))}
      </div>

      <div className="p-4 border-t border-space-secondary/30">
        <Button 
          variant="ghost" 
          className="w-full flex items-center justify-center text-space-text-primary hover:bg-space-secondary/50 hover:text-space-danger"
          onClick={handleLogout}
        >
          <LogOut className={`h-5 w-5 ${!isOpen && 'mx-auto'}`} />
          {isOpen && <span className="ml-2">Logga ut</span>}
        </Button>
      </div>
    </aside>
  );
};

export default DashboardSidebar;
