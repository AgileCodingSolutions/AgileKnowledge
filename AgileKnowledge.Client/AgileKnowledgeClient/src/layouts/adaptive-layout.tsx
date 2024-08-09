import { useState, useEffect } from 'react';

interface IAdaptiveLayoutProps {
    DesktopPage: React.ComponentType;
}

export default function AdaptiveLayout({ DesktopPage }: IAdaptiveLayoutProps) {
  
    const PageComponent = DesktopPage;

    return <PageComponent />;
}


