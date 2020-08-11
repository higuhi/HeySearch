import React from 'react';

/**
 * React component that makes a safe link tag which opens a new window 
 * with rel="noopener noreferrer" option. This component will use the 
 * same props as standard anchor (A) tag (e.g. href).
 * @param {Object} props - react props
 */
const ExternalLink = (props) => {
    return (
        <a {...props} target="_blank" rel="noopener noreferrer">
        {props.children}
        </a>
    );
};

export default ExternalLink;