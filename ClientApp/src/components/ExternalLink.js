import React from 'react';

/**
 * Makes a safe link to open a new window by specifing rel="noopener noreferrer".
 * Use the same props for anchor (a) element (e.g. href)
 * @param {*} props 
 */
const ExternalLink = (props) => {
    return (
        <a {...props} target="_blank" rel="noopener noreferrer">
        {props.children}
        </a>
    );
};

export default ExternalLink;