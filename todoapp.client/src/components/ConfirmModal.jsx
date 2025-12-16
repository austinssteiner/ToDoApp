export default function ConfirmModal({ 
  isOpen, 
  title, 
  message, 
  confirmText = 'Delete', 
  cancelText = 'Cancel',
  onConfirm, 
  onCancel,
  loading = false 
}) {
  if (!isOpen) return null;

  return (
    <div className="modal-overlay" onClick={onCancel}>
      <div className="modal-content" onClick={(e) => e.stopPropagation()}>
        <h2>{title}</h2>
        <p>{message}</p>
        <div className="modal-actions">
          <button 
            onClick={onCancel} 
            className="btn-secondary"
            disabled={loading}
          >
            {cancelText}
          </button>
          <button 
            onClick={onConfirm} 
            className="btn-danger"
            disabled={loading}
          >
            {loading ? 'Deleting...' : confirmText}
          </button>
        </div>
      </div>
    </div>
  );
}

